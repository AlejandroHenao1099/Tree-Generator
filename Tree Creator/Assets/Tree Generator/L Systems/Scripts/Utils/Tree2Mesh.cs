using System.Collections.Generic;
using UnityEngine;
using LindenmayerSystems;

public class Tree2Mesh
{
    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    int resolution = 5;
    float prevRadius;

    public Mesh Create(TreeNode tree, int resolutionCilinder = 5)
    {
        var mesh = new Mesh();
        vertices = new List<Vector3>();
        triangles = new List<int>();
        resolution = resolutionCilinder;

        var direction = (tree.Child.Position - tree.Position).normalized;
        prevRadius = tree.Radius;
        CreateCircle(tree.Position, direction, tree.Radius);
        CreateBranch(tree);

        mesh.SetVertices(vertices);
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
        return mesh;
    }

    private void CreateBranch(TreeNode currentNode)
    {
        var direction = Vector3.zero;
        if (currentNode.Child == null && !currentNode.hasBrothers)
        {
            CloseBranch(currentNode.Position);
            return;
        }
        else if (currentNode.Child == null && currentNode.hasBrothers)
        {
            direction = (currentNode.Position - currentNode.GetParent().Position).normalized;
            UpdateVertices(currentNode.Position, direction, currentNode.Radius);
        }

        if (currentNode.Child != null)
        {
            direction = (currentNode.Child.Position - currentNode.Position).normalized;
            UpdateVertices(currentNode.Position, direction, currentNode.Radius, prevRadius);
            prevRadius = currentNode.Radius;
            CreateBranch(currentNode.Child);
        }
        if (currentNode.hasBrothers)
        {
            for (int i = 0; i < currentNode.BrotherCount; i++)
            {
                direction = (currentNode.GetBrother(i).Position - currentNode.Position).normalized;
                CreateCircle(currentNode.Position, direction, currentNode.Radius);
                prevRadius = currentNode.Radius;
                CreateBranch(currentNode.GetBrother(i));
            }
        }
    }

    private void UpdateVertices(Vector3 center, Vector3 axis, float radius, float prevRadius = float.MaxValue)
    {
        var currentIndex = vertices.Count - resolution;
        CreateCircle(center, axis, radius, prevRadius);
        UpdateTriangles(currentIndex);
    }

    private void CreateCircle(Vector3 center, Vector3 axis, float radius, float prevRadius = float.MaxValue)
    {
        if (prevRadius != float.MaxValue)
        {
            var min = Mathf.Min(radius, prevRadius);
            radius = Mathf.Lerp(0, min, 0.84f);
        }
        float step = 360f / (float)resolution;
        var perpendicular = Vector3.Slerp(axis, -axis, 0.5f);
        for (int i = 0; i < resolution; i++)
        {
            var newPoint = Quaternion.AngleAxis(step * i, axis) * (perpendicular * radius);
            vertices.Add(center + newPoint);
        }
    }

    private void UpdateTriangles(int currentVertexIndex)
    {
        int vi = currentVertexIndex;
        for (int j = 0; j < resolution - 1; j++, vi++)
            SetQuad(vi, resolution);

        SetTriangle(vi, vi - (resolution - 1), vi + resolution);
        SetTriangle(vi - (resolution - 1), vi + 1, vi + resolution);
    }

    private void CloseBranch(Vector3 position)
    {
        var vi = vertices.Count - resolution;
        vertices.Add(position);
        int finalPoint = vertices.Count - 1;
        for (int i = 0; i < resolution - 1; i++, vi++)
            SetTriangle(vi, vi + 1, finalPoint);

        SetTriangle(vi, vi - (resolution - 1), finalPoint);
    }

    private void SetQuad(int currentIndex, int resolution)
    {
        SetTriangle(currentIndex, currentIndex + 1, currentIndex + resolution);
        SetTriangle(currentIndex + 1, currentIndex + resolution + 1, currentIndex + resolution);
    }

    private void SetTriangle(int a, int b, int c)
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
