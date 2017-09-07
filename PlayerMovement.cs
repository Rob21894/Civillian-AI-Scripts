using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {


    PlayerControl playerControl;
    public Transform playerAim;
    public float velocity;
    public bool holsterDelay = true;
    public bool toggleRun = false;
    [SerializeField]
    float pistolIdleLowTimer = 10.0f;



    float xDirection;
    public float yDirection;
    public GameObject detectedObject = null;

    Animation playerAnimation;
    // Use this for initialization
    void Start ()
    {
        playerControl = GetComponent<PlayerControl>();
        playerAnimation = GetComponent<Animation>();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (playerControl.playerStates == PlayerControl.PlayerStates.Holstered || playerControl.playerStates == PlayerControl.PlayerStates.Unholstered)
        {
            LookAtMousePos();
            playerControl.anim.speed = 1;
            if (playerControl.anim == null) return;
            if (!Input.GetKey(KeyCode.LeftShift))
            {
                xDirection = Input.GetAxis("Horizontal");
                if (yDirection > 1.0f)
                {
                    yDirection -= 1.0f * Time.deltaTime;
                }
                else
                {
                    yDirection = Input.GetAxis("Vertical");
                }

                if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
                {
                    if (!playerControl.anim.GetBool("Crouch"))
                    {
                        playerControl.anim.SetBool("Crouch", true);
                        
                    }
                    else if (playerControl.anim.GetBool("Crouch"))
                    {
                        playerControl.anim.SetBool("Crouch", false);
                    }

                }
                GetDirection(xDirection, yDirection);
            }
            else if (playerControl.anim.GetFloat("velocityY") >= .9f && Input.GetKey(KeyCode.LeftShift))
            {
                //  Debug.Log("Running");
                float xDirection = Input.GetAxis("Horizontal");
                if (!playerControl.anim.GetBool("Crouch"))
                {
                    if (yDirection < 2)
                    {
                        yDirection += 1.0f * Time.deltaTime;

                    }
                }
                else
                {
                    playerControl.anim.speed = 2;
                }


                GetDirection(xDirection, yDirection);

            }
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Detectwall();
            }
        }
        else if (playerControl.playerStates == PlayerControl.PlayerStates.Sneaking)
        {
            
            playerControl.anim.SetBool("WallSneak", true);
            xDirection = Input.GetAxis("Horizontal");
            yDirection = Input.GetAxis("Vertical");
            transform.rotation = Quaternion.Euler(CheckAngle().x, CheckAngle().y, CheckAngle().z);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playerControl.anim.SetBool("WallSneak", false);
                detectedObject = null;
                playerControl.playerStates = PlayerControl.PlayerStates.Holstered;
         
            }
            GetDirection(xDirection, yDirection);

        }
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            HolsterDelay();
        }   
    }


    public void Detectwall()
    {
        RaycastHit hit;
       // velocity = GetComponent<Rigidbody>().velocity.sqrMagnitude;
        Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward * .5f);
       // Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -transform.forward * 10.0f);
       // Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.right * 10.0f);
       // Debug.DrawRay(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -transform.right * 10.0f);
        if (detectedObject == null)
        {
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.forward, out hit, .5f, 1 << 9, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.tag == "Wall")
                {
                    // Debug.Log(hit.point);
                    detectedObject = hit.collider.transform.gameObject;
                    transform.RotateAround(transform.position, transform.up, 180.0f);
                    // transform.rotation = Quaternion.Euler(transform.rotation.x, 180.0f, transform.rotation.y);
                    // transform.rotation = Quaternion.Inverse(detectedObject.transform.rotation);

                }
            }
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -transform.forward, out hit, .5f, 1 << 9, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.tag == "Wall")
                {
                    detectedObject = hit.collider.transform.gameObject;
                    transform.RotateAround(transform.position, transform.up, 0.0f);
                    //Debug.Log("hit wall backward");
                    // Debug.Log(hit.point);
                    // detectedObject = hit.collider.gameObject;
                    // Debug.Log(detectedObject.name);
                }
            }
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), transform.right, out hit, .5f, 1 << 9, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.tag == "Wall")
                {
                    //  Debug.Log(hit.point);
                    detectedObject = hit.collider.transform.gameObject;
                    transform.RotateAround(transform.position, transform.up, 270f);
                    // Debug.Log("hit wall right");
                    // detectedObject = hit.collider.gameObject;
                }

            }
            if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 1.0f, transform.position.z), -transform.right, out hit, .5f, 1 << 9, QueryTriggerInteraction.Collide))
            {
                if (hit.collider.tag == "Wall")
                {
                    // Debug.Log(hit.point);
                    detectedObject = hit.collider.transform.gameObject;
                    transform.RotateAround(transform.position, transform.up, 90.0f);
                    // Debug.Log("hit wall");
                    // detectedObject = hit.collider.gameObject;
                }
            }
            Invoke("CheckAngle", 1f);

            if (detectedObject != null)
            {
                playerControl.playerStates = PlayerControl.PlayerStates.Sneaking;
            }


        }
        else
        {
            
            // transform.rotation = Quaternion.Euler(-transform.up * detectedObject.transform.eulerAngles.y);
        }

    }
    
    public void GetDirection(float x, float y)
    {
        playerControl.anim.SetFloat("velocityX", x);
        playerControl.anim.SetFloat("velocityY", y);
    }

    public bool ToggleRun()
    {
        return false;
    }
    public void HolsterDelay()
    {

        if (playerControl.anim.GetBool("Holstered"))
        {
            playerControl.playerStates = PlayerControl.PlayerStates.Holstered;
            playerControl.anim.SetBool("Holstered", false);
           
        }
        else
        {
            playerControl.playerStates = PlayerControl.PlayerStates.Unholstered;
            playerControl.anim.SetBool("Holstered", true);
        }
    }

    public bool DetectMouseClick()
    {
        float tempX;
        float actualX;
        actualX = Input.mousePosition.x;
        tempX = actualX;

        if (!Input.GetMouseButtonDown(0) || !Input.GetMouseButtonDown(1) || !Input.GetMouseButtonDown(2) || !Input.anyKey)
        {
            return true;
        }
        else
            return false;
    }
    public void LookAtMousePos()
    {
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit mousehit;

        if (Physics.Raycast(mouseRay,out mousehit, 100, 1 << 11))
        {
            playerAim.transform.position = mousehit.point;

           // transform.LookAt(new Vector3(0, playerAim.transform.position.y, playerAim.transform.position.z));
        }
        if (Vector3.Distance(transform.position, playerAim.transform.position) > 0.5f)
        {
            Quaternion lookRotation;
            Vector3 direction;
            direction = (playerAim.transform.position - transform.position).normalized;
            lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100);
        }
    }
    public Vector3 CheckAngle()
    {
        Vector3 targetRot = transform.eulerAngles;
        targetRot.x = Mathf.Round(targetRot.x / 90) * 90;
        targetRot.y = Mathf.Round(targetRot.y / 90) * 90;
        targetRot.z = Mathf.Round(targetRot.z / 90) * 90;
        transform.eulerAngles = targetRot;
        return targetRot;

        /* ensures all angles are rounded up to the nearest 90 degrees */
    }
    public string GetCurrentAnimation()
    {
        string cur = null;

        if (playerControl.anim.GetCurrentAnimatorStateInfo(0).IsName("crouched_walking"))
        {
            cur = "crouched_walking";
        }

        return cur;
    }


    public void OldCode() // remove when blend tree states are finished
    {
        //if (Input.GetKeyDown(KeyCode.W) && playerControl.anim.GetBool("Idle"))
        //{
        //    playerControl.playerStates = PlayerControl.PlayerStates.Walking;
        //    //Debug.Log("Down");
        //}
        //if (playerControl.playerStates == PlayerControl.PlayerStates.idle)
        //{
        //   // if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        //   // {
        //        if (!playerControl.anim.GetBool("Idle"))
        //        {
        //        playerControl.anim.SetBool("Walking", false);
        //        playerControl.anim.SetBool("Running", false);
        //        playerControl.anim.SetBool("Idle", true);
        //        playerControl.anim.SetBool("PistolIdle", false);
        //        playerControl.anim.SetBool("PistolIdleLow", false);
        //        playerControl.anim.SetBool("PistolWalk", false);
        //        playerControl.anim.SetBool("PistolRun", false);
        //    }

        //    if (Input.GetKeyDown(KeyCode.Mouse2) && !playerControl.anim.GetBool("PistolIdle") && !holsterDelay)
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolIdle;
        //        Invoke("HolsterDelay", .3f);
        //    }
        //}

        //if (playerControl.playerStates == PlayerControl.PlayerStates.Walking)
        //{
        //    if (Input.GetKey(KeyCode.W) && !playerControl.anim.GetBool("Walking"))
        //    {
        //        playerControl.anim.SetBool("Walking", true);
        //        playerControl.anim.SetBool("Running", false);
        //        playerControl.anim.SetBool("Idle", false);
        //        playerControl.anim.SetBool("PistolIdle", false);
        //        playerControl.anim.SetBool("PistolIdleLow", false);
        //        playerControl.anim.SetBool("PistolWalk", false);
        //        playerControl.anim.SetBool("PistolRun", false);
        //    }

        //    if (Input.GetKey(KeyCode.LeftShift))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.Running;
        //    }
        //    else if (Input.GetKeyUp(KeyCode.W))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.idle;
        //    }


        //    if (Input.GetKeyDown(KeyCode.Mouse2) && !playerControl.anim.GetBool("PistolWalk") && !holsterDelay)
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolWalk;
        //        Invoke("HolsterDelay", .3f);
        //    }

        //}


        //if (playerControl.playerStates == PlayerControl.PlayerStates.Running)
        //{
        //    if (Input.GetKey(KeyCode.W) && !playerControl.anim.GetBool("Running"))
        //    {
        //        playerControl.anim.SetBool("Walking", false);
        //        playerControl.anim.SetBool("Running", true);
        //        playerControl.anim.SetBool("Idle", false);
        //        playerControl.anim.SetBool("PistolIdle", false);
        //        playerControl.anim.SetBool("PistolIdleLow", false);
        //        playerControl.anim.SetBool("PistolWalk", false);
        //        playerControl.anim.SetBool("PistolRun", false);
        //    }

        //    if (Input.GetKeyUp(KeyCode.LeftShift))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.Walking;
        //    }
        //    else if (Input.GetKeyUp(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.idle;
        //    }
        //    else if (Input.GetKeyUp(KeyCode.W))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.idle;
        //    }

        //}



        //// Gun states


        //if (playerControl.playerStates == PlayerControl.PlayerStates.PistolIdle)
        //{
        //    playerControl.anim.SetBool("Walking", false);
        //    playerControl.anim.SetBool("Running", false);
        //    playerControl.anim.SetBool("Idle", false);
        //    playerControl.anim.SetBool("PistolIdle", true);
        //    playerControl.anim.SetBool("PistolIdleLow", false);
        //    playerControl.anim.SetBool("PistolWalk", false);
        //    playerControl.anim.SetBool("PistolRun", false);

        //    if (Input.GetKeyDown(KeyCode.W) && playerControl.anim.GetBool("PistolIdle"))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolWalk;
        //    }
        //    if (Input.GetKeyDown(KeyCode.Mouse2) && !playerControl.anim.GetBool("Idle") && holsterDelay)
        //    {
        //        Debug.Log("Running");
        //        Invoke("HolsterDelay", .3f);
        //        playerControl.playerStates = PlayerControl.PlayerStates.idle;
        //    }

        //    //if (DetectMouseClick())
        //    //{
        //    //    pistolIdleLowTimer -= 1.0f * Time.deltaTime;

        //    //    if (pistolIdleLowTimer <= 0.0f)
        //    //    {
        //    //        playerControl.anim.SetBool("PistolIdle", false);
        //    //        playerControl.anim.SetBool("PistolIdleLow", true);
        //    //    }
        //    //}
        //    //if (!DetectMouseClick() && playerControl.anim.GetBool("PistolIdleLow"))
        //    //{
        //    //    pistolIdleLowTimer = 10.0f;
        //    //    playerControl.anim.SetBool("PistolIdle", true);
        //    //    playerControl.anim.SetBool("PistolIdleLow", false);

        //    //}
        //}

        //if (playerControl.playerStates == PlayerControl.PlayerStates.PistolWalk)
        //{
        //    if (Input.GetKey(KeyCode.W) && !playerControl.anim.GetBool("PistolWalk"))
        //    {
        //        playerControl.anim.SetBool("Walking", false);
        //        playerControl.anim.SetBool("Running", false);
        //        playerControl.anim.SetBool("Idle", false);
        //        playerControl.anim.SetBool("PistolIdle", false);
        //        playerControl.anim.SetBool("PistolIdleLow", false);
        //        playerControl.anim.SetBool("PistolWalk", true);
        //        playerControl.anim.SetBool("PistolRun", false);
        //    }

        //    if (Input.GetKeyDown(KeyCode.LeftShift))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolRun;
        //    }
        //    else if (Input.GetKeyUp(KeyCode.W))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolIdle;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Mouse2) && holsterDelay)
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.Walking;
        //        Invoke("HolsterDelay", .3f);
        //    }
        //}

        //if (playerControl.playerStates == PlayerControl.PlayerStates.PistolRun)
        //{
        //    if (Input.GetKey(KeyCode.W) && !playerControl.anim.GetBool("PistolRun"))
        //    {
        //        playerControl.anim.SetBool("Walking", false);
        //        playerControl.anim.SetBool("Running", false);
        //        playerControl.anim.SetBool("Idle", false);
        //        playerControl.anim.SetBool("PistolIdle", false);
        //        playerControl.anim.SetBool("PistolWalk", false);
        //        playerControl.anim.SetBool("PistolIdleLow", false);
        //        playerControl.anim.SetBool("PistolRun", true);
        //    }

        //    if (Input.GetKeyUp(KeyCode.LeftShift))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolWalk;
        //    }
        //    if (Input.GetKeyUp(KeyCode.W))
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.PistolIdle;
        //    }
        //    else if (Input.GetKeyDown(KeyCode.Mouse2) && holsterDelay)
        //    {
        //        playerControl.playerStates = PlayerControl.PlayerStates.Running;
        //        Invoke("HolsterDelay", .3f);
        //    }
        //}
    }
}
