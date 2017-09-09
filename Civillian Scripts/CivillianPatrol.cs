using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

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
	        civillianControl.anim.SetBool("Walking", true);
	        updatedWaypoint = true;
        }
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
	    if (civillianControl.NavAgent.enabled)
	    {
	        remainingDistance = civillianControl.NavAgent.remainingDistance;
        }
        if (civillianControl.civillianStatus == CivillianControl.CivillianStatus.Alive)
	    {
            // Handles all states
	        switch (civillianControl.civillianState)
	        {
	            case CivillianControl.CivillianState.Patrolling:
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

	                if (isFleeing && civillianControl.NavAgent.remainingDistance < 1.0f && updatedWaypoint)
	                {
	                    civillianControl.StartCoroutine(civillianControl.CivilianCoolDown(10.0f));
                        isFleeing = false;	                  
	                }
	                break;
	            case CivillianControl.CivillianState.Investigate:
                    break;
	            case CivillianControl.CivillianState.AlertingGuard:
                    if (civillianControl.NavAgent.remainingDistance < 1.5f && civillianControl.alertingGuard)
	                {
	                    civillianControl.StartCoroutine(civillianControl.CivilianCoolDown(10.0f));
	                    civillianControl.alertingGuard = false;
	                }
                    break;
	            default:
	                throw new ArgumentOutOfRangeException();
	        }
	    }

    }

    public void InitializeValues()
    {
        // stores all values in suitable arrays
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
        // handles simple patrolling logic between waypoints
        if (civillianWaypointSettings.Length == 0)
        {
            return;
        }

        if (civillianControl.NavAgent.remainingDistance < 1f && civillianControl.anim.GetBool("Walking") && updatedWaypoint)
        {
            civillianControl.anim.SetBool("Walking", false);
            civillianControl.NavAgent.isStopped = false;
            StartCoroutine(Idle());
        }
    }

    public void ChooseNextPoint()
    {
        currentWaypoint = (currentWaypoint + 1) % wayPoints.Length; // chooses next waypoint from array
        civillianControl.NavAgent.SetDestination(wayPoints[currentWaypoint].transform.position); // updates destination
        civillianControl.anim.SetBool("Walking", true); // enables walking animation
        Invoke("WaypointUpdated",.6f); // updates waypoint boolean to true after .6 seconds

    }

    private void ChooseExitPoint()
    {
        StopAllCoroutines();
        civillianControl.NavAgent.ResetPath();
        int rand = Random.Range(0, exitPoints.Length);
        civillianControl.NavAgent.SetDestination(exitPoints[rand].transform.position);
        civillianControl.anim.SetBool("Alive", true);
        civillianControl.anim.SetBool(IdleAnim,false);
        civillianControl.anim.SetBool("Idle", true);
        civillianControl.anim.SetBool("Walking", true);
        civillianControl.anim.SetBool("Running", true);
        Invoke("WaypointUpdated",.5f);

    }

    public void IsFleeing()
    {
        if (isFleeing)
        {
            isFleeing = false;
        }
        else
        {
            isFleeing = true;
        }
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
        float waitTime = IdleTimes[currentWaypoint]; // stores correct waittime for waypoint
        GetAnimation();  
  
        while (!isIdle)
        {
            // Rotates player to target roation
            if (civillianWaypointSettings[currentWaypoint].targetRotation != null) // check is rotation exists
            {
                Vector3 direction =  (civillianWaypointSettings[currentWaypoint].targetRotation.position - transform.position).normalized;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y,0);
            }

            waitTime -= 1.0f * Time.deltaTime;
            if (waitTime <= 0)
            {
                if (!civillianControl.anim.IsInTransition(0) && civillianControl.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f) // waits for animation to finish cycle before moving
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
