using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class CivillianPatrol : MonoBehaviour
{

    public float remainingDistance;
    public enum IdleAnimation
    {
        Idle,
        Smoke,
        MakeCall
    };

    [System.Serializable]
    public class CivillianWaypointSettings
    {
        [Tooltip("Set a String/Number to identify waypoint in heirarchy")]
        public string WaypointNumber;
        public GameObject wayPoint;
        [Tooltip("Set a Target Rotation for the idle of the way point (Not Essential)")]
        public Transform targetRotation;
        public float IdleTime;
        public IdleAnimation IdleAnimation;
    }

    public CivillianWaypointSettings[] civillianWaypointSettings;
    private GameObject[] wayPoints;
    private bool updatedWaypoint = false;
    private float[] IdleTimes;
    private int currentWaypoint = 0;
    public GameObject[] exitPoints;
    private bool isFleeing = false;
    public string IdleAnim;

    private CivillianControl civillianControl;
	// Use this for initialization
	void Start ()
	{
	    civillianControl = GetComponent<CivillianControl>();
        InitializeValues();
        exitPoints = new GameObject[GameObject.FindGameObjectsWithTag("Exit Point").Length];
        exitPoints = GameObject.FindGameObjectsWithTag("Exit Point");
	    if (wayPoints.Length > 1)
	    {
	        civillianControl.NavAgent.SetDestination(wayPoints[currentWaypoint].transform.position);
	        Debug.Log(wayPoints[0].transform.position);
	        civillianControl.anim.SetBool("Walking", true);
	        updatedWaypoint = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (civillianControl.civillianStatus == CivillianControl.CivillianStatus.Alive)
	    {

	        switch (civillianControl.civillianState)
	        {
	            case CivillianControl.CivillianState.Patrolling:
	                remainingDistance = civillianControl.NavAgent.remainingDistance;
                    Patrolling();
	                break;
	            case CivillianControl.CivillianState.Scared:
	                Scared();
	                break;
	            case CivillianControl.CivillianState.Fleeing:
	                if (!isFleeing)
	                {
	                    ChooseExitPoint();
	                    isFleeing = true;
	                }
	                break;
	        }
	    }

    }

    public void InitializeValues()
    {
        wayPoints = new GameObject[civillianWaypointSettings.Length];
        IdleTimes = new float[civillianWaypointSettings.Length];
        for (int i = 0; i < civillianWaypointSettings.Length; i++)
        {
            wayPoints[i] = civillianWaypointSettings[i].wayPoint;
            IdleTimes[i] = civillianWaypointSettings[i].IdleTime;
        }
    }
    public void Patrolling()
    {
        if (civillianWaypointSettings.Length == 0)
        {
           // Debug.LogError("Assign Waypoints");
            return;
        }

        if (civillianControl.NavAgent.remainingDistance < 1f && civillianControl.anim.GetBool("Walking") && updatedWaypoint)
        {
            civillianControl.anim.SetBool("Walking", false);
            civillianControl.NavAgent.isStopped = false;
            StartCoroutine(Idle());
        }
    }

    private void ChooseNextPoint()
    {
        currentWaypoint = (currentWaypoint + 1) % wayPoints.Length;
        civillianControl.NavAgent.SetDestination(wayPoints[currentWaypoint].transform.position);
        civillianControl.anim.SetBool("Walking", true);
        Invoke("WaypointUpdated",.6f);

    }

    private void ChooseExitPoint()
    {
        int rand = Random.Range(0, exitPoints.Length);
        civillianControl.NavAgent.SetDestination(exitPoints[rand].transform.position);
        //civillianControl.SetAllAnimatorStatesToFalse();
        civillianControl.NavAgent.isStopped = false;
        civillianControl.NavAgent.ResetPath();
        civillianControl.anim.SetBool("Alive", true);
        civillianControl.anim.SetBool("Idle", true);
        civillianControl.anim.SetBool(IdleAnim,false);
        civillianControl.anim.SetBool("Walking", true);
        civillianControl.anim.SetBool("Running", true);

    }

    public void WaypointUpdated()
    {
        updatedWaypoint = true;
    }

    IEnumerator Idle()
    {
        updatedWaypoint = false;
        bool isIdle = false;
        civillianControl.anim.SetBool("Idle", true);
        float delay = 0.5f;
        float waitTime = IdleTimes[currentWaypoint];
        GetAnimation();
       

        while (!isIdle)
        {
            if (civillianWaypointSettings[currentWaypoint].targetRotation != null)
            {
                Vector3 direction =  (civillianWaypointSettings[currentWaypoint].targetRotation.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y,0);
            }
            waitTime -= 1.0f * Time.deltaTime;
            if (waitTime <= 0)
            {
                if (!civillianControl.anim.IsInTransition(0) && civillianControl.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    civillianControl.anim.SetBool(IdleAnim, false);
                    delay -= 1.0f * Time.deltaTime;
       
                    if (delay <= 0)
                    {
                        ChooseNextPoint();
                        civillianControl.anim.SetBool("Idle", true);
                        isIdle = true;
                    }
                }
            }

            yield return null;
        }
    }

    private void GetAnimation()
    {
        IdleAnim = civillianWaypointSettings[currentWaypoint].IdleAnimation.ToString();
        civillianControl.anim.SetBool(IdleAnim, true);
    }

    bool AnimatorIsPlaying()
    {
        return civillianControl.anim.GetCurrentAnimatorStateInfo(0).length >
               civillianControl.anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }



    public void Scared()
    {
        civillianControl.SetAllAnimatorStatesToFalse();
        civillianControl.NavAgent.isStopped = false;
        civillianControl.NavAgent.ResetPath();
        civillianControl.anim.SetBool("Alive", true);
        civillianControl.anim.SetBool("Idle", true);
        civillianControl.anim.SetBool("Scared", true);
    }

}
