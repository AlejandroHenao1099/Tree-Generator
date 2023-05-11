using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BranchOnSurface : MonoBehaviour
{
    public int sections;
    public int resolution;
    public float radio;

    public Transform target;
    public bool verticesOnSurface = true;

    public int resolutionBase;
    public int sectionsBase;
    public float radioBase;

    private Mesh mesh;
    private List<Vector3> verticesList;
    private Vector3[] vertices;
    private Vector3[] normals;
    private List<int> trianglesList;

    private Collider colliderTarget;
    public Material material;

    void Start()
    {
        colliderTarget = target.GetComponent<Collider>();
        CreateBranch();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    void CreateBranch()
    {
        mesh = new Mesh();
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        CreateMainBranch();
        // CreateBase();

        CreateTrianglesBranch();
        vertices = verticesList.ToArray();
        mesh.vertices = vertices;
        mesh.SetTriangles(trianglesList, 0);
        mesh.RecalculateNormals();
        normals = mesh.normals;
    }

    private void CreateMainBranch()
    {
        var step = (2 * Mathf.PI) / (float)resolution;
        var stepRadius = 1f / (float)(sections - 1);
        var stepDistance = 2f / (float)(sections - 1);
        for (int i = 0; i < sections; i++)
        {
            var currentRadio = Mathf.Lerp(0.1f, radio, 1 - (stepRadius * i));
            for (int j = 0; j < resolution; j++)
            {
                var newPos = new Vector3(Mathf.Cos(step * j) * currentRadio, Mathf.Sin(step * j) * currentRadio, i * stepDistance);
                verticesList.Add(newPos);
            }
        }
    }

    private void CreateBase()
    {
        var step = (2 * Mathf.PI) / (float)resolution;
        var stepRadius = 1f / (float)(sections - 1);
        var stepDistance = 1f / (float)(sections - 1);
        for (int i = 0; i < sections; i++)
        {
            var currentRadio = Mathf.Lerp(0.05f, radio, Mathf.Pow(1 - (stepRadius * i), 2));
            for (int j = 0; j < resolution; j++)
            {
                var newPos = new Vector3(Mathf.Cos(step * j) * currentRadio, Mathf.Sin(step * j) * currentRadio, i * stepDistance - (stepDistance * 1));
                verticesList.Add(newPos);
            }
        }
    }

    void Update()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        var newPosition = colliderTarget.ClosestPoint(transform.position);
        var normal = (newPosition - target.position).normalized;
        transform.forward = normal;
        transform.position = newPosition;
        if (verticesOnSurface)
            UpdatePositionVertices();
    }

    private void UpdatePositionVertices()
    {
        var step = (2 * Mathf.PI) / (float)resolution;
        var stepDistance = 1f / (float)(sections - 1);
        for (int j = 0; j < resolution; j++)
        {
            var oldPos = new Vector3(Mathf.Cos(step * j) * radio, Mathf.Sin(step * j) * radio, -stepDistance * 1);
            var newPos = colliderTarget.ClosestPoint(transform.TransformPoint(oldPos));
            var newNormal = (newPos - target.position).normalized;
            newPos = transform.InverseTransformPoint(newPos);
            newNormal = transform.InverseTransformDirection(newNormal);
            vertices[j] = newPos;
            normals[j] = newNormal;
        }
        mesh.vertices = vertices;
        mesh.normals = normals;
    }

    private void CreateTrianglesBranch()
    {
        int vi = 0;
        // for (int i = 0; i < sections - 2; i++, vi++)
        for (int i = 0; i < sections - 1; i++, vi++)
        {
            for (int j = 0; j < resolution - 1; j++, vi++)
                SetQuad(vi, resolution);

            SetTriangle(vi, vi - (resolution - 1), vi + resolution);
            SetTriangle(vi - (resolution - 1), vi + 1, vi + resolution);
        }

        // int finalPoint = vi + resolution;
        // for (int i = 0; i < resolution - 1; i++, vi++)
        //     SetTriangle(vi, vi + 1, finalPoint);

        // SetTriangle(vi, vi - (resolution - 1), finalPoint);
    }

    private void SetQuad(int currentIndex, int resolution)
    {
        SetTriangle(currentIndex, currentIndex + 1, currentIndex + resolution);
        SetTriangle(currentIndex + 1, currentIndex + resolution + 1, currentIndex + resolution);
    }

    private void SetTriangle(int a, int b, int c)
    {
        trianglesList.Add(a);
        trianglesList.Add(b);
        trianglesList.Add(c);
    }

}
