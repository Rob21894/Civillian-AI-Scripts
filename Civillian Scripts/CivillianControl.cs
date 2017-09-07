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
    private bool isReacting = false;
    private bool soundDetected = false;
    private FieldOfView fov;
    private AIHearing aiHearing;
    private CivillianPatrol civPatrol;
	// Use this for initialization
	void Start ()
	{
	    fov = GetComponent<FieldOfView>();
	    aiHearing = GetComponent<AIHearing>();
	    NavAgent = GetComponent<NavMeshAgent>();
	    anim = GetComponent<Animator>();
	    civPatrol = GetComponent<CivillianPatrol>();
	    anim.SetBool("Alive", true);

    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        if (IsAlive(health) && civillianStatus != CivillianStatus.Dead && !isReacting)
        {
            //TargetDetection(fov.ReturnVisibleTargets());
            TargetLogic(ReturnClosestTarget(fov.visibleTargets.ToArray()));
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
        if (target != null && !isReacting)
        {
            if (target.tag == "Player")
            {
                //Debug.Log("Playerrrr");
            }
            else if (target.tag == "Gun")
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
                NavAgent.isStopped = false;
                NavAgent.ResetPath();
                isReacting = true;
            }
            else if (target.tag == "Civillian")
            {
                if (target.gameObject.GetComponent<CivillianControl>().civillianStatus == CivillianStatus.Dead)
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
                isReacting = true;
            }
           

        }
        else
        {
            isReacting = false;
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
            civPatrol.StopAllCoroutines();
            NavAgent.ResetPath();
            int randomNumber = Random.Range(1, 3);
            Debug.Log(randomNumber);
            if (randomNumber == 1)
            {
                civillianState = CivillianState.Fleeing;
            }
            else
            {
                Debug.Log("Pls");
                civillianState = CivillianState.Fleeing;
            }
        }
        else if (noise.ToUpper() == "BULLETHIT")
        {
            Debug.Log("Investigate");
        }

        Invoke("ResetSoundDetected",.5f);
        soundDetected = true;
    }

    public void ResetSoundDetected()
    {
        soundDetected = false;
    }
}
