using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class CivillianControl : MonoBehaviour
{
    public enum CivillianStatus
    {
        Alive,
        Dead
    }
    public enum CivillianState
    {
        Patrolling,
        Scared,
        Fleeing,
        AlertingGuard
    };

    public NavMeshAgent NavAgent;
    public Animator anim;
    public float health = 100f;
    public CivillianState civillianState;
    public CivillianStatus civillianStatus;
    private FieldOfView fov;

    private CivillianPatrol civPatrol;
	// Use this for initialization
	void Start ()
	{
	    fov = GetComponent<FieldOfView>();
	    NavAgent = GetComponent<NavMeshAgent>();
	    anim = GetComponent<Animator>();
	    civPatrol = GetComponent<CivillianPatrol>();
	    anim.SetBool("Alive", true);

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (IsAlive(health))
        {
            //TargetDetection(fov.ReturnVisibleTargets());
            TargetLogic(ReturnClosestTarget(fov.visibleTargets.ToArray()));
        }
        else
        {
            civillianStatus = CivillianStatus.Dead;
            SetAllAnimatorStatesToFalse();
            civPatrol.StopAllCoroutines();
            NavAgent.speed = 0;
            NavAgent.enabled = false;

        }
        if (civillianStatus == CivillianStatus.Dead)
        {
            anim.SetBool("Dead", true);
        }
		
	}


    bool IsAlive(float civHealth)
    {
        if (civHealth <= 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public Transform ReturnClosestTarget(Transform[] visibleTargets)
    {
        //get 3 closest characters (to referencePos)
        if (visibleTargets.Length == 0)
        {
            
        }
        else
        {
            Transform nClosest = visibleTargets.OrderBy(t => (t.position - transform.position).sqrMagnitude)
                .FirstOrDefault(); //or use .FirstOrDefault();  if you need just one

            return nClosest;
        }

        return null;


    }

    public void TargetLogic(Transform target)
    {
        if (target != null)
        {
            if (target.tag == "Player")
            {
                //Debug.Log("Playerrrr");
            }
            else if (target.tag == "Gun")
            {
                civillianState = CivillianState.Scared;
                NavAgent.isStopped = false;
                NavAgent.ResetPath();
                Debug.Log("OMG A GUN");
            }
        }
    }

    public void SetAllAnimatorStatesToFalse()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }
}
