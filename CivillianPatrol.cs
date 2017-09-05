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
        public float IdleTime;
        public IdleAnimation IdleAnimation;
    }

    public CivillianWaypointSettings[] civillianWaypointSettings;
    private GameObject[] wayPoints;
    private float[] IdleTimes;
    private int currentWaypoint = 0;
    public string IdleAnim;

    private CivillianControl civillianControl;
	// Use this for initialization
	void Start ()
	{
	    civillianControl = GetComponent<CivillianControl>();
        InitializeValues();
	    civillianControl.NavAgent.SetDestination(wayPoints[0].transform.position);
	    civillianControl.anim.SetBool("Walking", true);
        // ChooseNextPoint();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    remainingDistance = civillianControl.NavAgent.remainingDistance;
        
	    if (civillianControl.civillianState == CivillianControl.CivillianState.Patrolling)
	    {
	        Patrolling();
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
            Debug.LogError("Assign Waypoints");
            return;
        }

        if (civillianControl.NavAgent.remainingDistance < 1f && civillianControl.anim.GetBool("Walking"))
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
        Debug.Log("Run");
    }

    IEnumerator Idle()
    {
        bool isIdle = false;
        civillianControl.anim.SetBool("Idle", true);
        float delay = 0.5f;
        float waitTime = IdleTimes[currentWaypoint];
        Debug.Log(waitTime);
        GetAnimation();
       

        while (!isIdle)
        {
            waitTime -= 1.0f * Time.deltaTime;
            if (waitTime <= 0)
            {
                //civillianControl.anim.GetCurrentAnimatorStateInfo(0).IsName(IdleAnim)
                if (!civillianControl.anim.IsInTransition(0) && civillianControl.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
                {
                    Debug.Log("Rawr");
                    civillianControl.anim.SetBool(IdleAnim, false);
                    delay -= 1.0f * Time.deltaTime;
       
                    if (delay <= 0)
                    {
                        ChooseNextPoint();
                        civillianControl.anim.SetBool("Idle", false);
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

}
