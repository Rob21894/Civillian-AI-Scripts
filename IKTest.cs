using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTest : MonoBehaviour
{

    public GameObject LeftFootTarget;
    private Vector3 leftFootPos;
    public GameObject RightFootTarget;
    private Vector3 rightFootPos;


    public float Weight_Walking = 0.1f;
    private Animator anim;
	// Use this for initialization
	void Start ()
	{
	    anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
	{
	    RaycastHit lhit;
	    RaycastHit rhit;
        if (Physics.Raycast(LeftFootTarget.transform.position,-transform.up, out lhit, 1f, 1 << 11))
        {
            leftFootPos = lhit.point;
        }

	    if (Physics.Raycast(RightFootTarget.transform.position, -transform.up, out rhit, 1f, 1 << 11))
	    {
	        rightFootPos = rhit.point;
	    }

    }

    void OnAnimatorIK()
    {
            anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, Weight_Walking);
            anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, Weight_Walking);
            anim.SetIKPosition(AvatarIKGoal.RightFoot,RightFootTarget.transform.position);
            anim.SetIKPosition(AvatarIKGoal.LeftFoot, LeftFootTarget.transform.position);
    }
}
