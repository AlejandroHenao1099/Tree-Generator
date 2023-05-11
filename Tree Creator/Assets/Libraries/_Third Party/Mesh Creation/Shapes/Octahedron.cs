using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public static class Octahedron
    {
        private static int subdivisions = 1;
        private static Vector3[] vertices;
        private static int[] triangles;

        private static Vector3[] directions = {
            Vector3.left,
            Vector3.back,
            Vector3.right,
            Vector3.forward
        };

        public static Mesh Create(int subdivisions)
        {
            if (subdivisions < 0)
            {
                subdivisions = 0;
                Debug.LogWarning("Octahedron Sphere subdivisions increased to minimum, which is 0.");
            }
            else if (subdivisions > 6)
            {
                subdivisions = 6;
                Debug.LogWarning("Octahedron Sphere subdivisions decreased to maximum, which is 6.");
            }
            Octahedron.subdivisions = subdivisions;
            Mesh mesh = new Mesh();
            mesh.name = "Octahedron";

            CreateOcthaedron();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        private static void CreateOcthaedron()
        {
            int resolution = 1 << subdivisions;
            vertices = new Vector3[(resolution + 1) * (resolution + 1) * 4 -
                (resolution * 2 - 1) * 3];
            triangles = new int[(1 << (subdivisions * 2 + 3)) * 3];


            int v = 0, vBottom = 0, t = 0;

            for (int i = 0; i < 4; i++)
            {
                vertices[v++] = Vector3.down;
            }

            for (int i = 1; i <= resolution; i++)
            {
                float progress = (float)i / resolution;
                Vector3 from, to;
                vertices[v++] = to = Vector3.Lerp(Vector3.down, Vector3.forward, progress);
                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.down, directions[d], progress);
                    t = CreateLowerStrip(i, v, vBottom, t);
                    v = CreateVertexLine(from, to, i, v);
                    vBottom += i > 1 ? (i - 1) : 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = resolution - 1; i >= 1; i--)
            {
                float progress = (float)i / resolution;
                Vector3 from, to;
                vertices[v++] = to = Vector3.Lerp(Vector3.up, Vector3.forward, progress);
                for (int d = 0; d < 4; d++)
                {
                    from = to;
                    to = Vector3.Lerp(Vector3.up, directions[d], progress);
                    t = CreateUpperStrip(i, v, vBottom, t);
                    v = CreateVertexLine(from, to, i, v);
                    vBottom += i + 1;
                }
                vBottom = v - 1 - i * 4;
            }

            for (int i = 0; i < 4; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = v;
                triangles[t++] = ++vBottom;
                vertices[v++] = Vector3.up;
            }
            NormalizeVertices();
        }

        private static int CreateVertexLine(Vector3 from, Vector3 to, int steps, int v)
        {
            for (int i = 1; i <= steps; i++)
            {
                vertices[v++] = Vector3.Lerp(from, to, (float)i / steps);
            }
            return v;
        }

        private static int CreateLowerStrip(int steps, int vTop, int vBottom, int t)
        {
            for (int i = 1; i < steps; i++)
            {
                triangles[t++] = vBottom;
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;

                triangles[t++] = vBottom++;
                triangles[t++] = vTop++;
                triangles[t++] = vBottom;
            }
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = vTop;
            return t;
        }

        private static int CreateUpperStrip(int steps, int vTop, int vBottom, int t)
        {
            triangles[t++] = vBottom;
            triangles[t++] = vTop - 1;
            triangles[t++] = ++vBottom;
            for (int i = 1; i <= steps; i++)
            {
                triangles[t++] = vTop - 1;
                triangles[t++] = vTop;
                triangles[t++] = vBottom;
                triangles[t++] = vBottom;
                triangles[t++] = vTop++;
                triangles[t++] = ++vBottom;
            }
            return t;
        }

        private static void NormalizeVertices()
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i] = vertices[i].normalized;
        }
    }
}