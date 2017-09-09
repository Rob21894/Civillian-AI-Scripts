using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using Random = UnityEngine.Random;

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
        Investigate,
        AlertingGuard
    };

    public NavMeshAgent NavAgent;
    public Animator anim;
    public float health = 100f;
    public CivillianState civillianState;
    public CivillianStatus civillianStatus;
    private bool isReacting = false;
    private bool soundDetected = false;
    [HideInInspector] public bool alertingGuard = false;


    private FieldOfView fov;
    private AIHearing aiHearing;
    private CivillianPatrol civPatrol;
    private CivilianSuspicionControl suspicionControl;
	// Use this for initialization
	void Start ()
	{
	    fov = GetComponent<FieldOfView>();
	    aiHearing = GetComponent<AIHearing>();
	    NavAgent = GetComponent<NavMeshAgent>();
	    anim = GetComponent<Animator>();
	    civPatrol = GetComponent<CivillianPatrol>();
	    suspicionControl = GetComponent<CivilianSuspicionControl>();
	    anim.SetBool("Alive", true);

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (IsAlive(health) && civillianStatus != CivillianStatus.Dead && !isReacting)
        {
            TargetLogic(ReturnClosestTarget(fov.visibleTargets.ToArray()));
            if (suspicionControl.playerDetected)
            {
                ChooseAction();
                suspicionControl.playerDetected = false;
            }
        }
        
        if (!IsAlive(health))
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

        if (civillianState == CivillianState.Patrolling)
        {
            if (aiHearing.NoiseMade() != null && !soundDetected)
            {
                NoiseLogic(aiHearing.NoiseMade());
            }
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
        //Returns closest targets (to referencePos)
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
        if (target != null && !isReacting)
        {
            if (target.tag == "Player")
            {
                //Debug.Log("Playerrrr");
                if (target.GetComponent<PlayerControl>().anim.GetBool("Holstered"))
                {
                    if (!suspicionControl.isSuspicious)
                    {
                        suspicionControl.isSuspicious = true;
                        suspicionControl.StartCoroutine(suspicionControl.SuspicionDetection());
                    }
                }
                else
                {
                    suspicionControl.isSuspicious = false;
                    suspicionControl.suspicionText.text = "";
                }
            }
            else if (target.tag == "Gun")
            {
                ChooseAction();
                NavAgent.isStopped = false;
                NavAgent.ResetPath();
                isReacting = true;
            }
            else if (target.tag == "Civillian")
            {
                if (target.gameObject.GetComponent<CivillianControl>().civillianStatus == CivillianStatus.Dead)
                {
                    ChooseAction();
                }
                isReacting = true;
            }
           

        }
        else
        {
            isReacting = false;
        }
    }

    public GameObject DetectClosestGuard()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, 30.0f, 1 << 9);
        List<GameObject> guards = new List<GameObject>();


        if (col.Length > 0)
        {
            foreach (Collider c in col)
            {
                RaycastHit hit;
                if (Physics.Linecast(transform.position + Vector3.up * 2.0f, c.transform.position, out hit,
                    1 << 10))
                {
                    // hit wall
                }
                else
                {
                    if (c.gameObject.tag == "Guard")
                    {
                        guards.Add(c.gameObject);
                    }
                }
            }
        }

        if (guards.Count > 0)
        {
            GameObject getClosestGuard = guards.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
                .FirstOrDefault();

            return getClosestGuard;

        }

        return null;
    }

    public void ChooseAction()
    {
        if (DetectClosestGuard() != null)
        {
            NavAgent.ResetPath();
            NavAgent.isStopped = false;
            civPatrol.StopAllCoroutines();
            SetAllAnimatorStatesToFalse();
            NavAgent.SetDestination(DetectClosestGuard().transform.position);
            anim.SetBool("Alive", true);
            anim.SetBool("Idle", true);
            anim.SetBool("Walking",true);
            anim.SetBool("Running", true);
            //Invoke(Alerting(true),.5f);
            Invoke("Alerting",.5f);
            civillianState = CivillianState.AlertingGuard;

        }
        else
        {
            Debug.Log("No Guard Detected");
            ChooseRandomState();
        }

    }

    public void Alerting()
    {
        if (alertingGuard)
        {
            alertingGuard = false;
        }
        else
        {
            alertingGuard = true;
        }
    }

    public void ChooseRandomState()
    {
        int randomNumber = Random.Range(1, 3);
        if (randomNumber == 1)
        {
            civillianState = CivillianState.Scared;
        }
        else
        {
            civillianState = CivillianState.Fleeing;
        }

    }
    public void SetAllAnimatorStatesToFalse()
    {
        foreach (AnimatorControllerParameter parameter in anim.parameters)
        {
            anim.SetBool(parameter.name, false);
        }
    }

    public void NoiseLogic(string noise)
    {
        if (noise.ToUpper() == "GUN")
        {
            ChooseAction();
        }
        else if (noise.ToUpper() == "BULLETHIT")
        {
            Debug.Log("Investigate");
        }
        else if (noise.ToUpper() == "OBSTACLEOBJECT")
        {
            
        }

        Invoke("ResetSoundDetected",.5f);
        soundDetected = true;
    }

    public void ResetSoundDetected()
    {
        soundDetected = false;
    }

    public IEnumerator CivilianCoolDown(float timeToWait)
    {
        NavAgent.isStopped = false;
        NavAgent.ResetPath();
        SetAllAnimatorStatesToFalse();
        anim.SetBool("Alive", true);
        anim.SetBool("Idle", true);
        bool isCooledDown = false;

        while (!isCooledDown)
        {
            yield return new WaitForSeconds(timeToWait);
            civillianState = CivillianState.Patrolling;
            civPatrol.ChooseNextPoint();
            isCooledDown = true;

        }
    }
}
