using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CivilianSuspicionControl : MonoBehaviour
{

    public TextMesh suspicionText;
    public float suspiciousMeter;
    public bool isSuspicious = false;
    public bool playerDetected = false;

    private CivillianControl civillianControl;
	// Use this for initialization
	void Start ()
	{
	    suspicionText.text = "";
        civillianControl = GetComponent<CivillianControl>();
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public IEnumerator SuspicionDetection()
    {
        float suspicionProgress = 0.0f;
        bool startSuspicion = false;
        suspicionText.text = "?";
        while (isSuspicious && suspicionProgress < 100.0f && !startSuspicion)
        {
            suspicionProgress += 25.0f * Time.deltaTime;
            suspiciousMeter = suspicionProgress;

            if (suspicionProgress >= 100.0f)
            {
                suspicionText.text = "!";
                playerDetected = true;
            }
            yield return null;

        }
    }

    public void NoiseHeard() //logic for objects such as chairs/boxes/fruit
    {
        
    }
}
