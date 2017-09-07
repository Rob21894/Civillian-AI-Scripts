using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    // public enum PlayerStates { idle, Walking, Running, PistolIdle, PistolWalk, PistolRun, Paused};
    public float playerHealth;
    public enum PlayerStates { Unholstered,Holstered, Sneaking, Paused };
    public PlayerStates playerStates;
    [HideInInspector]
    public Animator anim;

	// Use this for initialization
	void Start ()
    {
        playerStates = PlayerStates.Holstered;
        anim = GetComponent<Animator>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
