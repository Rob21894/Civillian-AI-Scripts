using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;

public class TestLogic : MonoBehaviour {

	// Use this for initialization

   //public GameObject[] Objects = new GameObject[8];
   //private List<GameObject> tempObjects = new List<GameObject>();
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

    //IEnumerator SelectRandomObject()
    //{
    //    for (int i = 0; i < Objects.Length; i++)
    //    {
    //        tempObjects.Add(Objects[i]); // add all items into list
    //    }

    //    while (true)
    //    {
    //        yield return new WaitForSeconds(5.0f);
    //        GameObject SelectedObject = tempObjects[Random.Range(0, tempObjects.Count)];
    //        tempObjects.Remove(SelectedObject);


    //        // if you want to list to be empty before restoring
    //        if (tempObjects.Count <= 0)
    //        {
    //            tempObjects.Clear();
    //            for (int i = 0; i < Objects.Length; i++)
    //            {
    //                tempObjects.Add(Objects[i]); // add all items into list
    //            }
    //        }

    //        // if you want to restore it at a set value
    //        if (tempObjects.Count <= 4)
    //        {
    //            tempObjects.Clear();
    //            for (int i = 0; i < Objects.Length; i++)
    //            {
    //                tempObjects.Add(Objects[i]); // add all items into list
    //            }
    //        }
    //    }
    //}
}
