using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLogic : MonoBehaviour {

	// Use this for initialization
    private GameObject[] civs;
	void Start ()
	{
        civs = new GameObject[GameObject.FindGameObjectsWithTag("Civillian").Length];
	    civs = GameObject.FindGameObjectsWithTag("Civillian");
	    for (int i = 0; i < civs.Length; i++)
	    {
	        if (civs[i].GetComponent<CivillianControl>() != null)
	        {
	            civs[i].GetComponent<CivillianControl>().NavAgent.avoidancePriority =
	                civs[i].GetComponent<CivillianControl>().NavAgent.avoidancePriority += i;

	        }
	    }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int randomNumber = Random.Range(1, 3);
            Debug.Log(randomNumber);
        }


		
	}
}
