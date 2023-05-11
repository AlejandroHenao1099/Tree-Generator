using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{

    public static class RoundedCube
    {        
        private static int xRes, yRes, zRes;
        private static int roundness;

        private static Vector3[] vertices;
        private static Vector3[] normals;
        private static int[] triangles;

        private static Vector3 center;
        private static Matrix4x4 scalerUp, scalerDown;
        // private static Vector2[] uvs;

        public static Mesh Create(int xSize, int ySize, int zSize, int roundness,
            float width = 1f, float height = 1f, float depth = 1f)
        {
            RoundedCube.xRes = xSize;
            RoundedCube.yRes = ySize;
            RoundedCube.zRes = zSize;
            RoundedCube.roundness = roundness;
            center = new Vector3((float)xRes * 0.5f, (float)yRes * 0.5f, (float)zRes * 0.5f);
            scalerUp = scalerDown = new Matrix4x4();
            scalerDown.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(1f / (float)xRes, 1f / (float)yRes, 1f / (float)zRes));
            scalerUp.SetTRS(Vector3.zero, Quaternion.identity, new Vector3(width, height, depth));

            var mesh = new Mesh();
            mesh.name = "Rounded Cube";

            CreateVertices();
            CreateTriangles();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void CreateVertices()
        {
            int cornerVertices = 8;
            int edgeVertices = (xRes + yRes + zRes - 3) * 4;
            int faceVertices = ((xRes - 1) * (yRes - 1) +
                                    (xRes - 1) * (zRes - 1) +
                                    (yRes - 1) * (zRes - 1)) * 2;
            vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
            normals = new Vector3[vertices.Length];
            // uvs = new Vector2[vertices.Length];
            int v = 0;
            for (int y = 0; y <= yRes; y++)
            {
                for (int x = 0; x <= xRes; x++)
                    SetVertex(v++, x, y, 0);
                for (int z = 1; z <= zRes; z++)
                    SetVertex(v++, xRes, y, z);
                for (int x = xRes - 1; x >= 0; x--)
                    SetVertex(v++, x, y, zRes);
                for (int z = zRes - 1; z > 0; z--)
                    SetVertex(v++, 0, y, z);
            }

            for (int z = 1; z < zRes; z++)
            {
                for (int x = 1; x < xRes; x++)
                    SetVertex(v++, x, yRes, z);
            }
            for (int z = 1; z < zRes; z++)
            {
                for (int x = 1; x < xRes; x++)
                    SetVertex(v++, x, 0, z);
            }
        }

        private static void SetVertex(int i, int x, int y, int z)
        {
            Vector3 inner = vertices[i] = new Vector3(x, y, z);
            if (x < roundness)
                inner.x = roundness;
            else if (x > xRes - roundness)
                inner.x = xRes - roundness;
            if (y < roundness)
                inner.y = roundness;
            else if (y > yRes - roundness)
                inner.y = yRes - roundness;
            if (z < roundness)
                inner.z = roundness;
            else if (z > zRes - roundness)
                inner.z = zRes - roundness;

            normals[i] = (vertices[i] - inner).normalized;
            vertices[i] = (inner + normals[i] * roundness) - center;
            vertices[i] = scalerDown.MultiplyPoint3x4(vertices[i]);
            vertices[i] = scalerUp.MultiplyPoint3x4(vertices[i]);


            // Vector3 v = new Vector3(x, y, z) * 2f / xSize - Vector3.one;
            // float x2 = v.x * v.x;
            // float y2 = v.y * v.y;
            // float z2 = v.z * v.z;
            // Vector3 s;
            // s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
            // s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
            // s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

            // normals[i] = s;
            // vertices[i] = normals[i] * ((float)xSize / 2f);
        }

        private static void CreateTriangles()
        {
            int quads = (xRes * yRes + xRes * zRes + yRes * zRes) * 2;
            // int[] triangles = new int[quads * 6];
            triangles = new int[quads * 6];
            int ring = (xRes + zRes) * 2;
            int t = 0, v = 0;
            for (int y = 0; y < yRes; y++, v++)
            {
                for (int q = 0; q < ring - 1; q++, v++)
                {
                    t = SetQuad(triangles, t, v, v + 1, v + ring, v + ring + 1);
                }
                t = SetQuad(triangles, t, v, v - ring + 1, v + ring, v + 1);
            }
            t = CreateTopFace(triangles, t, ring);
            t = CreateBottomFace(triangles, t, ring);
        }

        private static int CreateTopFace(int[] triangles, int t, int ring)
        {
            int v = ring * yRes;
            for (int x = 0; x < xRes - 1; x++, v++)
            {
                t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + ring);
            }
            t = SetQuad(triangles, t, v, v + 1, v + ring - 1, v + 2);

            int vMin = ring * (yRes + 1) - 1;
            int vMid = vMin + 1;
            int vMax = v + 2;

            for (int z = 1; z < zRes - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid, vMin - 1, vMid + xRes - 1);
                for (int x = 1; x < xRes - 1; x++, vMid++)
                {
                    t = SetQuad(
                    triangles, t,
                    vMid, vMid + 1, vMid + xRes - 1, vMid + xRes);
                }
                t = SetQuad(triangles, t, vMid, vMax, vMid + xRes - 1, vMax + 1);
            }

            int vTop = vMin - 2;
            t = SetQuad(triangles, t, vMin, vMid, vTop + 1, vTop);

            for (int x = 1; x < xRes - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, vTop, vTop - 1);
            }
            t = SetQuad(triangles, t, vMid, vTop - 2, vTop, vTop - 1);
            return t;
        }

        private static int CreateBottomFace(int[] triangles, int t, int ring)
        {
            int v = 1;
            int vMid = vertices.Length - (xRes - 1) * (zRes - 1);
            t = SetQuad(triangles, t, ring - 1, vMid, 0, 1);
            for (int x = 1; x < xRes - 1; x++, v++, vMid++)
            {
                t = SetQuad(triangles, t, vMid, vMid + 1, v, v + 1);
            }
            t = SetQuad(triangles, t, vMid, v + 2, v, v + 1);
            int vMin = ring - 2;
            vMid -= xRes - 2;
            int vMax = v + 2;
            for (int z = 1; z < zRes - 1; z++, vMin--, vMid++, vMax++)
            {
                t = SetQuad(triangles, t, vMin, vMid + xRes - 1, vMin + 1, vMid);
                for (int x = 1; x < xRes - 1; x++, vMid++)
                {
                    t = SetQuad(
                    triangles, t,
                    vMid + xRes - 1, vMid + xRes, vMid, vMid + 1);
                }
                t = SetQuad(triangles, t, vMid + xRes - 1, vMax + 1, vMid, vMax);
            }
            int vTop = vMin - 1;
            t = SetQuad(triangles, t, vTop + 1, vTop, vTop + 2, vMid);
            for (int x = 1; x < xRes - 1; x++, vTop--, vMid++)
            {
                t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vMid + 1);
            }
            t = SetQuad(triangles, t, vTop, vTop - 1, vMid, vTop - 2);
            return t;
        }

        private static int SetQuad(int[] triangles, int i, int v00, int v10, int v01, int v11)
        {
            triangles[i] = v00;
            triangles[i + 1] = triangles[i + 4] = v01;
            triangles[i + 2] = triangles[i + 3] = v10;
            triangles[i + 5] = v11;
            return i + 6;
        }
    }
}