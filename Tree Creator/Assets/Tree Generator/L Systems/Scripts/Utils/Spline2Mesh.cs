using System.Collections.Generic;
using UnityEngine;

//Script for create a mesh like a branch
public static class Spline2Mesh
{
    public static Mesh Create(Splines spline, List<float> widths, int resolutionCilinder = 5)
    {
        var resolutionSpline = spline.PointsCount;
        float step = 1f / (float)(resolutionSpline - 1);
        var points = new Vector3[resolutionSpline];
        var directions = new Vector3[resolutionSpline];

        for (int i = 0; i < resolutionSpline; i++)
        {
            points[i] = spline.GetPoint(i * step);
            directions[i] = spline.GetDirection(i * step);
        }
        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var triangles = new List<int>();

        // for (int i = 0; i < resolutionSpline - 1; i++)
        for (int i = 0; i < resolutionSpline; i++)
            CreateCircle(points[i], directions[i], resolutionCilinder, vertices, LerpWidth(widths, step * i));
        // CreateCircle(points[i], directions[i], resolutionCilinder, vertices, widths[i]);
        // vertices.Add(points[points.Length - 1]);

        CreateTriangles(vertices, triangles, resolutionSpline, resolutionCilinder);

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        // mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    private static void CreateCircle(Vector3 center, Vector3 axis, int resolutionCircle, List<Vector3> vertices, float width)
    {
        float step = 360f / (float)resolutionCircle;
        var perpendicular = Vector3.Slerp(axis, -axis, 0.5f);
        for (int i = 0; i < resolutionCircle; i++)
        {
            var newPoint = Quaternion.AngleAxis(step * i, axis) * (perpendicular * width);
            vertices.Add(center + newPoint);
        }
    }

    private static void CreateTriangles(List<Vector3> vertices, List<int> triangles, int resolution, int circleResolution)
    {
        int vi = 0;
        // for (int i = 0; i < resolution - 2; i++, vi++)
        for (int i = 0; i < resolution - 1; i++, vi++)
        {
            for (int j = 0; j < circleResolution - 1; j++, vi++)
                SetQuad(triangles, vi, circleResolution);

            SetTriangle(triangles, vi, vi - (circleResolution - 1), vi + circleResolution);
            SetTriangle(triangles, vi - (circleResolution - 1), vi + 1, vi + circleResolution);
        }

        // int finalPoint = vi + circleResolution;
        // for (int i = 0; i < circleResolution - 1; i++, vi++)
        //     SetTriangle(triangles, vi, vi + 1, finalPoint);

        // SetTriangle(triangles, vi, vi - (circleResolution - 1), finalPoint);
    }

    private static void SetQuad(List<int> triangles, int currentIndex, int resolution)
    {
        SetTriangle(triangles, currentIndex, currentIndex + 1, currentIndex + resolution);
        SetTriangle(triangles, currentIndex + 1, currentIndex + resolution + 1, currentIndex + resolution);
    }

    private static void SetTriangle(List<int> triangles, int a, int b, int c)
    {
        triangles.Add(a);
        triangles.Add(b);
        triangles.Add(c);
    }

    private static float LerpWidth(List<float> widths, float t)
    {
        // var len = widths.Count - 1;
        // var index = Mathf.RoundToInt(Mathf.Lerp(0, len, t));
        // return widths[index];
        var end = widths.Count - 1;
        return Mathf.Lerp(widths[0], widths[end], Mathf.Clamp01(t));
    }
}
