using System.Collections.Generic;
using UnityEngine;

public class PruebaRamificacionMesh : MonoBehaviour
{
    public int cantidadCirculos;
    public int resolution;
    public int distanciaOrigen = 1;
    public float radio;
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<int> primeros;

    public Material material;

    public Vector3 axis;
    public Vector3 point;

    private int cantidadIteracionesBajas = 2;
    public float gradosAxis = 90;

    private void Start()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        primeros = new List<int>();
        var mesh = new Mesh();
        HallarIteraciones();
        CreateCirclesAround();
        switch (cantidadCirculos)
        {
            case 2:
                CreateTriangles2();
                break;
            case 3:
                CreateTriangles3();
                break;
            case 4:
                CreateTriangles2();
                break;
            default:

                break;
        }
        // CreateTriangles();

        mesh.SetVertices(vertices);
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = material;
    }

    private void HallarIteraciones()
    {
        var counter = 0;
        for (int i = 3; i <= resolution; i++)
            if ((i & (1 << 0)) == 1)
                counter++;
        cantidadIteracionesBajas = counter;
    }

    private void CreateCirclesAround()
    {
        CreateCircle(Vector3.zero, Vector3.up, radio);
        float step = (2 * Mathf.PI) / (float)cantidadCirculos;
        for (int i = 0; i < cantidadCirculos; i++)
        {
            var newPos = new Vector3(Mathf.Cos(step * i), 0, Mathf.Sin(step * i));
            newPos = (newPos + Vector3.up).normalized * distanciaOrigen;
            CreateCircle(newPos, newPos.normalized, radio);
        }
    }

    private void CreateCircle(Vector3 center, Vector3 axis, float radius)
    {
        float step = 360f / (float)resolution;
        for (int i = 0; i < resolution; i++)
        {
            var newPoint = RotateAroundCirle(axis, step * i, radius);
            vertices.Add(center + newPoint);
        }
        primeros.Add(vertices.Count - resolution);
    }

    private Vector3 RotateAroundCirle(Vector3 normal, float angle, float radius)
    {
        var newAxis = normal.normalized;
        var dirHor = new Vector3(newAxis.x, 0, newAxis.z).normalized;
        var perHor = Quaternion.Euler(0, 90, 0) * dirHor;
        perHor = perHor.normalized;

        var matrix = new Matrix4x4();
        var q = Quaternion.LookRotation(perHor, newAxis);
        matrix.SetTRS(Vector3.zero, q, Vector3.one);
        var radAng = angle * Mathf.Deg2Rad;
        return matrix.MultiplyPoint(new Vector3(Mathf.Sin(radAng), 0, Mathf.Cos(radAng)) * radius);
    }

    private void CreateTriangles2()
    {
        int middle = resolution % 2 == 0 ? resolution / 2 : (resolution - 1) / 2;
        var a = primeros[0];
        var b = primeros[1] + middle;
        var c = primeros[2];
        var d = 0;
        SetTriangle(a, b, c);

        var i = 0;

        for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        {
            a = IndexCircle(primeros[0], i, true);
            b = IndexCircle(primeros[2], i, true);
            c = IndexCircle(primeros[0], i + 1, true);
            d = IndexCircle(primeros[2], i + 1, true);
            SetQuad(a, b, c, d);
        }

        a = IndexCircle(primeros[0], i, true);
        b = IndexCircle(primeros[2], i, true);
        c = IndexCircle(primeros[1], 0, true);
        d = IndexCircle(primeros[2], i + 1, true);
        SetTriangle(a, b, c);

        for (int j = i, n = 0; i < j + resolution - cantidadIteracionesBajas - 1; i++, n++)
        {
            a = IndexCircle(primeros[0], i, true);
            b = IndexCircle(primeros[1], n, true);
            c = IndexCircle(primeros[0], i + 1, true);
            d = IndexCircle(primeros[1], n + 1, true);
            SetQuad(a, b, c, d);
        }



        i = 0;
        for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        {
            a = IndexCircle(primeros[1], middle - i, false);
            b = IndexCircle(primeros[1], middle - i - 1, false);
            c = IndexCircle(primeros[2], i, false);
            d = IndexCircle(primeros[2], i + 1, false);
            SetQuad(a, b, c, d);
        }
    }


    private void CreateTriangles3()
    {
        int middle = resolution % 2 == 0 ? resolution / 2 : (resolution - 1) / 2;
        int last = cantidadCirculos - 1;
        var a = IndexCircle(primeros[0], 1);
        var b = IndexCircle(primeros[0], 0);
        var c = IndexCircle(primeros[1], middle);
        var d = IndexCircle(primeros[2], 0);
        // SetQuad(a, b, c, d, true);



        var i = 0;
        // for (int n = 1; n < cantidadCirculos; n++)
        // {
        //     for (int i = 0; i < length; i++)
        //     {
        //         a = IndexCircle(primeros[0], 2, true);
        //         // b = IndexCircle(primeros[1], 0, true);
        //         b = IndexCircle(primeros[n], 0, true);
        //         c = IndexCircle(primeros[0], 3, true);
        //         SetTriangle(a, b, c, false);
        //     }
        // }



        a = IndexCircle(primeros[0], 2, true);
        b = IndexCircle(primeros[1], 0, true);
        c = IndexCircle(primeros[0], 3, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 3, true);
        b = IndexCircle(primeros[1], 0, true);
        c = IndexCircle(primeros[1], 1, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 3, true);
        b = IndexCircle(primeros[1], 1, true);
        c = IndexCircle(primeros[1], 2, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 3, true);
        b = IndexCircle(primeros[1], 2, true);
        c = IndexCircle(primeros[2], 0, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 3, true);
        b = IndexCircle(primeros[2], 0, true);
        c = IndexCircle(primeros[0], 4, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 0);
        b = IndexCircle(primeros[2], 0, true);
        c = IndexCircle(primeros[2], 1, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 0);
        b = IndexCircle(primeros[2], 1, true);
        c = IndexCircle(primeros[0], 1, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 1, true);
        b = IndexCircle(primeros[2], 1, true);
        c = IndexCircle(primeros[2], 2, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 1, true);
        b = IndexCircle(primeros[2], 2, true);
        c = IndexCircle(primeros[3], 0, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 1, true);
        b = IndexCircle(primeros[3], 0, true);
        c = IndexCircle(primeros[3], 1, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 1, true);
        b = IndexCircle(primeros[3], 1, true);
        c = IndexCircle(primeros[0], 2, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 2, true);
        b = IndexCircle(primeros[3], 1, true);
        c = IndexCircle(primeros[3], 2, true);
        SetTriangle(a, b, c, false);

        a = IndexCircle(primeros[0], 2, true);
        b = IndexCircle(primeros[3], 2, true);
        c = IndexCircle(primeros[1], 0, true);
        SetTriangle(a, b, c, false);

        return;


        i = 0;
        for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        {
            a = IndexCircle(primeros[1], middle - i, false);
            b = IndexCircle(primeros[1], middle - i - 1, false);
            c = IndexCircle(primeros[2], i, false);
            d = IndexCircle(primeros[2], i + 1, false);
            SetQuad(a, b, c, d);
        }
    }

    private void CreateTriangles()
    {
        int middle = resolution % 2 == 0 ? resolution / 2 : (resolution - 1) / 2;
        var a = primeros[0];
        var b = primeros[1];
        var c = primeros[cantidadCirculos - 1] + middle;
        var d = 0;
        // SetTriangle(a, b, c);

        var i = 0;
        // var inverse = cantidadCirculos - 1;
        // for (int k = 0; k < cantidadCirculos - 1; k++, i++)
        // {
        //     for (int j = i; i < j + resolution - cantidadIteracionesBajas - 1; i++)
        //     {
        //         a = IndexCircle(primeros[0], i, true);
        //         b = IndexCircle(primeros[inverse - k], i, true);
        //         c = IndexCircle(primeros[0], i + 1, true);
        //         d = IndexCircle(primeros[inverse - k], i + 1, true);
        //         SetQuad(a, b, c, d);
        //     }

        //     a = IndexCircle(primeros[0], i, true);
        //     b = IndexCircle(primeros[inverse - k], i, true);
        //     c = IndexCircle(primeros[inverse - k - 1], 0, true);
        //     // d = IndexCircle(primeros[inverse], i + 1, true);
        //     SetTriangle(a, b, c);
        // }

        var last = cantidadCirculos - 1;
        for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        {
            a = IndexCircle(primeros[0], i, true);
            b = IndexCircle(primeros[last], i, true);
            c = IndexCircle(primeros[0], i + 1, true);
            d = IndexCircle(primeros[last], i + 1, true);
            SetQuad(a, b, c, d);
        }

        a = IndexCircle(primeros[0], i, true);
        b = IndexCircle(primeros[last], i, true);
        c = IndexCircle(primeros[last - 1], 0, true);
        d = IndexCircle(primeros[2], i + 1, true);
        SetTriangle(a, b, c);

        for (int j = i, n = 0; i < j + resolution - cantidadIteracionesBajas - 1; i++, n++)
        {
            a = IndexCircle(primeros[0], i, true);
            b = IndexCircle(primeros[last - 1], n, true);
            c = IndexCircle(primeros[0], i + 1, true);
            d = IndexCircle(primeros[last - 1], n + 1, true);
            SetQuad(a, b, c, d);
        }



        // for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        // {
        //     a = IndexCircle(primeros[0], i, true);
        //     b = IndexCircle(primeros[2], i, true);
        //     c = IndexCircle(primeros[0], i + 1, true);
        //     d = IndexCircle(primeros[2], i + 1, true);
        //     SetQuad(a, b, c, d);
        // }

        // a = IndexCircle(primeros[0], i, true);
        // b = IndexCircle(primeros[2], i, true);
        // c = IndexCircle(primeros[1], 0, true);
        // d = IndexCircle(primeros[2], i + 1, true);
        // SetTriangle(a, b, c);

        // for (int j = i, n = 0; i < j + resolution - cantidadIteracionesBajas - 1; i++, n++)
        // {
        //     a = IndexCircle(primeros[0], i, true);
        //     b = IndexCircle(primeros[1], n, true);
        //     c = IndexCircle(primeros[0], i + 1, true);
        //     d = IndexCircle(primeros[1], n + 1, true);
        //     SetQuad(a, b, c, d);
        // }

        return;

        // var i = 0;
        // for (int k = 1; k <= cantidadCirculos; k++, i++)
        // {
        //     for (int j = i; i < j + cantidadIteracionesBajas; i++)
        //     {
        //         a = IndexCircle(primeros[0], i, true);
        //         b = IndexCircle(primeros[k], i, true);
        //         c = IndexCircle(primeros[0], i + 1, true);
        //         d = IndexCircle(primeros[k], i + 1, true);
        //         SetQuad(a, b, c, d);
        //     }
        //     if (k >= cantidadCirculos)
        //         continue;
        //     a = IndexCircle(primeros[0], i, true);
        //     b = IndexCircle(primeros[k], i, true);
        //     c = IndexCircle(primeros[0], i + 1, true);
        //     d = IndexCircle(primeros[k + 1], i + 1, true);
        //     SetQuad(a, b, c, d);
        // }

        i = 0;
        for (i = 0; i < resolution - cantidadIteracionesBajas - 1; i++)
        {
            a = IndexCircle(primeros[1], middle - i, false);
            b = IndexCircle(primeros[1], middle - i - 1, false);
            c = IndexCircle(primeros[2], i, false);
            d = IndexCircle(primeros[2], i + 1, false);
            SetQuad(a, b, c, d);
        }
    }

    private void SetQuad(int a, int b, int c, int d, bool invert = false)
    {
        if (invert)
        {
            SetTriangle(a, c, b);
            SetTriangle(b, c, d);
        }
        else
        {
            SetTriangle(a, b, c);
            SetTriangle(b, d, c);
        }
    }

    private void SetTriangle(int a, int b, int c, bool inverse = false)
    {
        if (inverse)
        {
            triangles.Add(a);
            triangles.Add(c);
            triangles.Add(b);
        }
        else
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
    }

    private int IndexCircle(int first, int index, bool inverse = false)
    {
        if (index < 0)
            index = resolution + index;
        index = index % resolution;
        if (inverse)
        {
            if (index == 0)
                return first;
            return first + (resolution - index);
        }
        return first + index;
    }

    private int InverseFirstCircle(int first)
    {
        var middle = (resolution - 1) / 2;
        return first + middle;
    }

    private void OnDrawGizmos()
    {
        // var newAxis = axis.normalized;
        // var dirHor = new Vector3(newAxis.x, 0, newAxis.z).normalized;
        // var perHor = Quaternion.Euler(0, 90, 0) * dirHor;
        // perHor = perHor.normalized;

        // var matrix = new Matrix4x4();
        // var q = Quaternion.LookRotation(perHor, newAxis);
        // matrix.SetTRS(Vector3.zero, q, Vector3.one);
        // var radAng = gradosAxis * Mathf.Deg2Rad;
        // var newPoint = matrix.MultiplyPoint(new Vector3(Mathf.Sin(radAng), 0, Mathf.Cos(radAng)));
        // // newPoint = newPoint.normalized;
        // Gizmos.color = Color.blue;
        // Gizmos.DrawLine(Vector3.zero, newAxis);
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawLine(newAxis, newAxis + perHor);
        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(newAxis, newAxis + newPoint);

        if (vertices != null)
        {
            // Gizmos.color = Color.red;
            for (int i = 0; i < vertices.Count; i++)
            {
                Gizmos.color = IfFirst(i) != true ? Color.red : Color.green;
                Gizmos.DrawCube(vertices[i], Vector3.one * 0.1f);
            }
        }
    }

    private bool IfFirst(int index)
    {
        for (int i = 0; i < primeros.Count; i++)
            if (index == primeros[i])
                return true;
        return false;
    }
}
