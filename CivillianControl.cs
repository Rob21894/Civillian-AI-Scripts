using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CivillianControl : MonoBehaviour
{

    public enum CivillianState
    {
        Patrolling,
        Hiding,
        Fleeing
    };

    public NavMeshAgent NavAgent;
    public Animator anim;
    public CivillianState civillianState;


	// Use this for initialization
	void Start ()
	{

	    NavAgent = GetComponent<NavMeshAgent>();
	    anim = GetComponent<Animator>();

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
