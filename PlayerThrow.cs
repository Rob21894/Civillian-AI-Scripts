using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerThrow : MonoBehaviour
{
    public Transform[] pointsTrans;
    public Transform throwSpawn;
    public GameObject currentThrowable;
    public float force;
    public float upforce;
    LineRenderer lineRen;
    public bool isThrowing = false;

    // Use this for initialization
    void Start()
    {
        



    }
    void Update()
    {
        if (isThrowing)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                GameObject throwable = Instantiate(currentThrowable, throwSpawn.position, Quaternion.identity);
                Vector3 targDirection = GetComponent<PlayerMovement>().playerAim.position - transform.position;
                targDirection.Normalize();
                force = CalculateForce(throwSpawn.transform.position, GetComponent<PlayerMovement>().playerAim.transform.position, 0f);
                throwable.GetComponent<Rigidbody>().AddForce(transform.up * upforce, ForceMode.Impulse);
                throwable.GetComponent<Rigidbody>().AddForce(targDirection * force, ForceMode.Impulse);

            }
            Vector3[] points = new Vector3[pointsTrans.Length];

            for (int i = 0; i < points.Length; i++)
            {
                points[i] = pointsTrans[i].position;
            }


            lineRen = gameObject.GetComponent<LineRenderer>();

            points = MakeSmoothCurve(points, 5.0f);

            lineRen.SetVertexCount(points.Length);

            for (int i = 0; i < points.Length; i++)
            {
                lineRen.SetPosition(i, points[i]);
            }
        }

    }
    //arrayToCurve is original Vector3 array, smoothness is the number of interpolations. 
    public static Vector3[] MakeSmoothCurve(Vector3[] arrayToCurve, float smoothness)
    {
        List<Vector3> points;
        List<Vector3> curvedPoints;
        int pointsLength = 0;
        int curvedLength = 0;

        if (smoothness < 1.0f) smoothness = 1.0f;

        pointsLength = arrayToCurve.Length;

        curvedLength = (pointsLength * Mathf.RoundToInt(smoothness)) - 1;
        curvedPoints = new List<Vector3>(curvedLength);

        float t = 0.0f;
        for (int pointInTimeOnCurve = 0; pointInTimeOnCurve < curvedLength + 1; pointInTimeOnCurve++)
        {
            t = Mathf.InverseLerp(0, curvedLength, pointInTimeOnCurve);

            points = new List<Vector3>(arrayToCurve);

            for (int j = pointsLength - 1; j > 0; j--)
            {
                for (int i = 0; i < j; i++)
                {
                    points[i] = (1 - t) * points[i] + t * points[i + 1];
                }
            }

            curvedPoints.Add(points[0]);
        }

        return (curvedPoints.ToArray());
    }

    public float CalculateForce(Vector3 startPos, Vector3 endPos, float extraForce)
    {
        float targDir = 0;
        targDir = Vector3.Distance(startPos, endPos);
        targDir += targDir * extraForce;

        return targDir;
    }
}