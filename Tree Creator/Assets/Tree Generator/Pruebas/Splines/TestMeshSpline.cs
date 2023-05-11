using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class TestMeshSpline : MonoBehaviour
{
    public Transform[] points;
    public int ver;
    public int hor;

    bool onStart;
    void Start()
    {
        var pointsVec = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            pointsVec[i] = points[i].position;
        var newSpline = new DynamicSplineMesh(pointsVec, ver, hor);
        newSpline.UpdateMesh();
        var mesh = newSpline.GetMesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
