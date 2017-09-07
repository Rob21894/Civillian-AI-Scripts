using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour {

    [Range(0, 50)]
    public float bulletSpeed;
    public float deactivateTimer;
    float tempDeactivateTimer;
    bool bulletHasHit = false; // bullet has hit something
    public float velocity;
    private MakeNoise makeNoise;
    void Start()
    {
        makeNoise = GetComponent<MakeNoise>();
        tempDeactivateTimer = deactivateTimer;
    }
    Vector3 prev = Vector3.zero;
    // Update is called once per frame

    private void Update()
    {
        if (prev == Vector3.zero)
        {
            prev = transform.position;
        }
        velocity = GetComponent<Rigidbody>().velocity.sqrMagnitude;
        if (deactivateTimer > 0)
        {
            deactivateTimer -= 1.0f * Time.deltaTime;
        }

        if (deactivateTimer <= 0)
        {
            deactivateTimer = tempDeactivateTimer;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
            prev = Vector3.zero;
            gameObject.SetActive(false);
        }

        Debug.DrawLine(prev, transform.position, Color.green);

    }
    void FixedUpdate()
    {
        if (bulletHasHit)
        {
            bulletHasHit = false;
        }

        if (!bulletHasHit)
        {
            BulletHitDetection();
            LookForCollision();
        }
        //  transform.Translate(transform.forward * bulletSpeed);

    }

    void LookForCollision()
    {
        Collider[] hitDetection = Physics.OverlapSphere(gameObject.transform.position, .1f, 1 << 9, QueryTriggerInteraction.Collide);
        //Collider[] hitDetection = Physics.SphereCast(new Ray(transform.position,Vector3.up),.1f,10.0f,1 << 9)

        if (hitDetection.Length > 0)
        {
            float closestObjectDist = 10000f;
            GameObject closestObj = hitDetection[0].gameObject;

            for (int i = 0; i < hitDetection.Length; i++)
            {
                if (Vector3.Distance(gameObject.transform.position, hitDetection[i].transform.position) < closestObjectDist)
                {
                    closestObjectDist = Vector3.Distance(gameObject.transform.position, hitDetection[i].gameObject.transform.position);
                    closestObj = hitDetection[i].gameObject;
                }
            }
            BulletHitObject(closestObj);
        }
    }
    public void BulletHitDetection()
    {
        RaycastHit hit;

        if (!bulletHasHit)
        {
            if (Physics.SphereCast(new Ray(transform.position, Vector3.up), .1f, 10.0f, 1 << 9))
            {
                
            }
            if (Physics.Linecast(prev,transform.position, out hit, 1 << 9, QueryTriggerInteraction.Collide))
            {
                BulletHitObject(hit.transform.gameObject);

            }
            

        }
    }

    public void BulletHitObject(GameObject hitObject)
    {
        bulletHasHit = true;
       // Debug.Log(bulletHasHit);
        if (hitObject.tag == "Wall")
        {
            makeNoise.noiseRadius = 10;
            makeNoise.makeNoise = true;
            Debug.Log("Hit Wall");

        }
        else if (hitObject.tag == "Civillian")
        {
            hitObject.GetComponent<CivillianControl>().health -= 200;
        }


        deactivateTimer = tempDeactivateTimer;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        this.gameObject.SetActive(false);
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, .1f);
    }
}
