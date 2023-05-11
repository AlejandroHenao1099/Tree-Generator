using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;
using static UnityEngine.Mathf;

public static class Points2Mesh
{
    public static Mesh Create(List<Vector3> pointsMesh, List<float> radiusList, int resolutionCilinder = 5, float defaultRadius = 1f)
    {
        if (radiusList == null)
        {
            radiusList = new List<float>();
            radiusList.Add(defaultRadius);
            radiusList.Add(defaultRadius);
        }
        var resolutionSpline = pointsMesh.Count;
        // var resolutionSpline = spline.PointsCount;
        float step = 1f / (float)(resolutionSpline - 1);
        var points = new Vector3[resolutionSpline];
        var directions = new Vector3[resolutionSpline];

        for (int i = 0; i < resolutionSpline; i++)
        {
            // points[i] = spline.GetPoint(i * step);
            points[i] = pointsMesh[i];
            // directions[i] = spline.GetDirection(i * step);

        }
        for (int i = 0; i < resolutionSpline - 1; i++)
            directions[i] = (pointsMesh[i + 1] - pointsMesh[i]).normalized;
        directions[resolutionSpline - 1] = directions[resolutionSpline - 2];

        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var triangles = new List<int>();
        var rotationSpline = new RotationSpline();


        for (int i = 0; i < resolutionSpline; i++)
            CreateFirstCircle(points[i], directions[i], resolutionCilinder, vertices, LerpWidth(radiusList, step * i), rotationSpline);

        CreateTriangles(vertices, triangles, resolutionSpline, resolutionCilinder);

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        // mesh.SetTriangles(triangles, 0);
        mesh.RecalculateNormals();
        return mesh;
    }

    private static void CreateFirstCircle(Vector3 center, Vector3 direction, int resolutionCircle, List<Vector3> vertices, float width, RotationSpline rotationSpline)
    {
        rotationSpline.Update(direction);
        float step = 360f / (float)resolutionCircle;
        for (int i = 0; i < resolutionCircle; i++)
        {
            var newPoint = new Vector3(Cos((i * step) * Deg2Rad), Sin((i * step) * Deg2Rad), 0) * width;
            newPoint = rotationSpline.GetPoint(newPoint);
            vertices.Add(center + newPoint);
        }
    }

    private static void CreateCircle(Vector3 center, Vector3 prevCenter, Vector3 direction, int resolutionCircle, List<Vector3> vertices, float width)
    {
        int startIndex = vertices.Count - 1 - (resolutionCircle - 1);
        UnityEngine.Plane plane = new UnityEngine.Plane(direction, center);
        for (int i = 0; i < resolutionCircle; i++, startIndex++)
        {
            var currentPoint = vertices[startIndex];
            var dirCurrentPoint = currentPoint - prevCenter;
            var newPoint = plane.ClosestPointOnPlane(center + dirCurrentPoint);
            vertices.Add(center + (newPoint - center).normalized);
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
