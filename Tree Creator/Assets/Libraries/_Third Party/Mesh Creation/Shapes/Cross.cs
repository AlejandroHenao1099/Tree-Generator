using UnityEngine;
using static UnityEngine.Mathf;

namespace MeshGenerator
{
    public static class Cross
    {
        public static Mesh Create(int resolution, float lenVer, float lenHor, float width, float angHor, float angVer)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Cross";
            mesh.vertices = CreateVertices(resolution, lenVer, lenHor, width, angVer, angHor);
            mesh.triangles = CreateTriangles(resolution);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Vector3[] CreateVertices(int resolution, float lenVer, float lenHor, float width, float angVer, float angHor)
        {
            Vector3[] vertices = new Vector3[5 + (4 * resolution)];

            vertices[0] = Vector3.zero;

            float angle = 0;
            float stepWidth = width / (float)(resolution - 1);
            for (int i = 0, k = 1; i < 4; i++)
            {
                float magnitude = i == 0 ? lenVer * 0.5f : i == 2 ? lenVer : lenHor;
                float angleRotation = i == 0 || i == 2 ? angle - angVer : angle - angHor;

                for (int j = 0; j < resolution; j++)
                {
                    vertices[k++] = Quaternion.Euler(0, 0, angleRotation) *
                        new Vector3(-(width * 0.5f) + (stepWidth * j), Random.Range(0.8f, 1.3f) * magnitude, 0);
                }
                vertices[k++] = Vector3.zero;
                angle -= 90;
            }

            int currI = 0;
            for (int n = 1; n <= 3; n++)
            {
                currI = n + (n * resolution);
                Vector3 prev = vertices[currI - 1];
                Vector3 next = vertices[currI + 1];

                Vector3 negPrev = -vertices[currI - resolution];
                Vector3 negNext = -vertices[currI + resolution];

                vertices[currI] = RecalcularPosicionEsquina(prev, next, negPrev, negNext);
            }
            currI = 4 + (4 * resolution);
            vertices[currI] = RecalcularPosicionEsquina(vertices[currI - 1], vertices[1],
                                    -vertices[currI - resolution], -vertices[1 + resolution]);
            return vertices;
        }

        private static Vector3 RecalcularPosicionEsquina(Vector3 prev, Vector3 next, Vector3 negPrev, Vector3 negNext)
        {
            float alpha = Vector3.Angle((prev - next).normalized, (negNext - next).normalized) * Mathf.Deg2Rad;

            float delta = Vector3.Angle((prev - negPrev).normalized, (next - negNext).normalized) * Mathf.Deg2Rad;
            float c = (next - prev).magnitude;

            float magA = (Mathf.Sin(alpha) * c) / Mathf.Sin(delta);
            return prev + ((negPrev - prev).normalized * magA);
        }

        private static int[] CreateTriangles(int resolution)
        {
            int lengthVertices = 5 + (4 * resolution);
            int[] triangles = new int[(2 + resolution - 1) * 3 * 4];
            int ti = 0;
            int vi = 1;
            for (vi = 1; vi < lengthVertices - 1; vi++, ti += 3)
            {
                triangles[ti] = 0;
                triangles[ti + 1] = vi;
                triangles[ti + 2] = vi + 1;
            }

            triangles[ti] = 0;
            triangles[ti + 1] = vi;
            triangles[ti + 2] = 1;
            return triangles;
        }

    }
}