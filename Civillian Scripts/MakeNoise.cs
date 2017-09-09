using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeNoise : MonoBehaviour
{
    public enum NoiseName
    {
        gun,
        footstep,
        bulletHit
    }

    public NoiseName noiseName;
    public bool makeNoise = false;
    public float noiseRadius;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (makeNoise)
        {
            StartNoise();
            makeNoise = false;
        }
		
	}

    public void StartNoise()
    {
        Collider[] NPCSNearby = Physics.OverlapSphere(gameObject.transform.position, noiseRadius, 1 << 9, QueryTriggerInteraction.Collide);
        List<GameObject> DetectedNPCS = new List<GameObject>();

        if (NPCSNearby.Length > 0)
        {
            for (int i = 0; i < NPCSNearby.Length; i++)
            {
                if (NPCSNearby[i].tag == "Civillian")
                {
                    DetectedNPCS.Add(NPCSNearby[i].gameObject);
                }
            }
        }

        CanBeHeard(DetectedNPCS.ToArray());
    }

    public void CanBeHeard(GameObject[] NPCS)
    {
        for (int i = 0; i < NPCS.Length; i++)
        {
            if (NPCS[i].tag == "Civillian")
            {
                NPCS[i].GetComponent<AIHearing>().NoiseDetected(this.transform.position, NPCS[i].GetComponent<CivillianControl>().gameObject.transform.position, noiseName);
            }
        }
    }

}
