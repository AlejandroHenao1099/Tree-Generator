using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeCreator;

public class TestSpline : MonoBehaviour
{
    public Transform[] points;
    public int resolutionSPline;
    public float sizeCube;

    // Spline spline;
    EvenSpline evenSpline;
    Color[] colorsArray = { Color.white, Color.red, Color.blue, Color.magenta, Color.black };

    private void Start()
    {
        var pointsVec = new Vector3[points.Length];
        for (int i = 0; i < 4; i++)
            pointsVec[i] = points[i].position;
        // evenSpline = new EvenSpline(pointsVec);
        // spline = new Spline(pointsVec);
    }

    // private void OnDrawGizmos()
    // {
    //     if (evenSpline == null) return;
    //     DivideCurveIntoSteps();

    //     float step = 1f / (float)(resolutionSPline - 1);
    //     for (int i = 0; i < resolutionSPline; i++)
    //     {
    //         var t = step * (float)i;
    //         Gizmos.DrawCube(spline.GetPoint(t), Vector3.one * sizeCube);
    //     }
    // }
}
