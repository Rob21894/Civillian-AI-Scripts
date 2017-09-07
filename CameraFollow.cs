using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {


    public GameObject player;
    public GameObject gameControl;
    public float cameraFollowSpeed;
    public float offset;
    public bool isSmoothFollow = true;
	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if (player)
        {
            Vector3 newCameraPosition = transform.position;
            newCameraPosition.x = player.transform.position.x;
            newCameraPosition.z = player.transform.position.z - offset;

            if (!isSmoothFollow)
            {
                transform.position = newCameraPosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, newCameraPosition, cameraFollowSpeed * Time.deltaTime);
            }
        }
	}
}
