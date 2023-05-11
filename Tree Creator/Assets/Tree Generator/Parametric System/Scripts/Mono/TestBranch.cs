using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBranch : MonoBehaviour
{
    public Transform[] points;
    private Branch branch;
    public float angleRotation;
    public Axis axisRotation;
    public int indiceStart;

    bool onStart = false;

    void Start()
    {
        Vector3[] pointsBranch = new Vector3[points.Length];
        for (int i = 0; i < points.Length; i++)
            pointsBranch[i] = points[i].position;

        branch = new Branch(pointsBranch, Vector3.up);
        onStart = true;
    }

    private void OnValidate()
    {
        if (onStart == false) return;

        var currPos = branch.Rotate(angleRotation, axisRotation, indiceStart);
        for (int i = 0; i < currPos.Length; i++)
            points[i].position = currPos[i];
    }
}
