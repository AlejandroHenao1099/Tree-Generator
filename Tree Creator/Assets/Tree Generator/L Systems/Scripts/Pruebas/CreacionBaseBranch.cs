using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreacionBaseBranch : MonoBehaviour
{
    public int sections;
    public int resolution;
    public float radio;

    public Transform target;
    public bool verticesOnSurface = true;

    public int resolutionBase;
    public int sectionsBase;
    public float radioBase;

    public float tamanoBase;

    private Mesh mesh;
    private List<Vector3> verticesList;
    private Vector3[] vertices;
    private Vector3[] normals;
    private List<int> trianglesList;

    private Collider colliderTarget;
    public Material material;

    public int currentIteration = 1;

    void Start()
    {
        mesh = new Mesh();
        verticesList = new List<Vector3>();
        trianglesList = new List<int>();

        CrearVertices();
        CreateTriangles();
        // vertices = verticesList.ToArray();
        // mesh.vertices = vertices;
        mesh.SetVertices(verticesList);
        mesh.SetTriangles(trianglesList, 0);
        mesh.RecalculateNormals();
        // normals = mesh.normals;
        // colliderTarget = target.GetComponent<Collider>();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    private void CrearVertices()
    {
        var highResolution = resolution + (sectionsBase * 2);
        var highRadius = radio * 4;
        var stepRadio = 1f / sectionsBase;
        var stepDistance = (float)tamanoBase / (float)sectionsBase;
        var n = 0;
        for (int i = 0; i < sectionsBase + 1; i++, n++)
        {
            var currRadio = Mathf.Lerp(radio, highRadius, 1 - Mathf.Sqrt(stepRadio * i));
            CrearCirculo(highResolution - (i * 2), currRadio, n * stepDistance);
        }
    }

    private void CrearCirculo(float resolution, float radio, float z)
    {
        var step = (2 * Mathf.PI) / (float)resolution;
        for (int j = 0; j < resolution; j++)
        {
            var newPos = new Vector3(Mathf.Cos(step * j) * radio, z, Mathf.Sin(step * j) * radio);
            verticesList.Add(newPos);
        }
    }

    private void CreateTriangles()
    {
        var currentResolution = 0;
        var vi = 0;
        var a = 0;
        var b = 0;
        var c = 0;
        var d = 0;

        var prevRes = currentResolution;

        for (int n = 0; n < currentIteration; n++, vi++)
        {
            prevRes += currentResolution;
            currentResolution = resolution + (sectionsBase * 2) - (n * 2);
            var middle = currentResolution / 2;
            var j = vi;
            for (int i = j; i < prevRes + middle; i++, vi++)
            {
                a = vi;
                b = vi + currentResolution;
                c = vi + 1;
                d = vi + currentResolution + 1;
                SetQuad(a, b, c, d);
            }
            a = vi;
            b = vi + currentResolution;
            c = vi + 1;
            d = vi + currentResolution + 1;
            SetTriangle(a, b, c);
            vi++;
            j = vi;
            for (int i = j; i < prevRes + currentResolution - (middle / 2); i++, vi++)
            {
                a = vi;
                b = vi + currentResolution - 1;
                c = vi + 1;
                d = vi + currentResolution;
                SetQuad(a, b, c, d);
            }
            a = vi;
            b = vi + currentResolution - 1;
            c = vi + 1;
            d = vi + currentResolution;
            SetTriangle(a, b, c);
            vi++;

            j = vi;
            for (int i = j; i < prevRes + currentResolution - 1; i++, vi++)
            {
                a = vi;
                b = vi + currentResolution - 2;
                c = vi + 1;
                d = vi + currentResolution - 1;
                SetQuad(a, b, c, d);
            }
            a = vi;
            b = vi + currentResolution - 2;
            c = vi - currentResolution + 1;
            d = vi + 1;
            SetQuad(a, b, c, d);
        }
    }

    private void SetQuad(int a, int b, int c, int d)
    {
        SetTriangle(a, b, c);
        SetTriangle(c, b, d);
    }

    private void SetTriangle(int a, int b, int c)
    {
        trianglesList.Add(a);
        trianglesList.Add(b);
        trianglesList.Add(c);
    }

    private void OnDrawGizmos()
    {
        if (verticesList != null)
        {
            Gizmos.color = Color.red;
            for (int i = 0; i < verticesList.Count; i++)
                Gizmos.DrawCube(verticesList[i], Vector3.one * 0.1f);
        }
    }
}
