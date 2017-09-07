using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIHearing : MonoBehaviour
{

    public float hearingDistance;
    [HideInInspector] public bool heardNoise = false;
    private string noiseName = null;


    public void NoiseDetected(Vector3 noisePos, Vector3 currentPos,Enum noise)
    {
        float distBetween = Vector3.Distance(noisePos, currentPos);

        if (distBetween <= hearingDistance)
        {
            heardNoise = true;
        }
        else
        {
            heardNoise = false;
        }

        noiseName = noise.ToString().ToUpper();
        Invoke("ResetNoise", .5f);
    }

    public string NoiseMade()
    {
        return noiseName;
    }

    public void ResetNoise()
    {
        noiseName = null;
    }

}
