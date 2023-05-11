using UnityEngine;
using static UnityEngine.Mathf;

namespace MeshGenerator
{
    public static class Cylinder
    {
        public static Mesh Create(int sides, int verticalResolution, float radius, float height)
        {
            var mesh = new Mesh();
            mesh.name = "Cylinder";

            mesh.vertices = CreateVertices(sides, verticalResolution, radius, height);
            mesh.triangles = CreateTriangles(sides, verticalResolution);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Vector3[] CreateVertices(int sides, int verticalResolution, float radius, float height)
        {
            int lenghtVertices = (sides * verticalResolution) + (sides * 2) + 2;
            var vertices = new Vector3[lenghtVertices];
            var center = new Vector3(0, height / 2f, 0);

            var k = 0;
            vertices[k++] = Vector3.zero - center;

            float stepAngleSides = (Mathf.PI * 2) / (float)sides;
            float stepHeight = height / (float)(verticalResolution - 1);

            for (int j = 0; j < sides; j++)
                vertices[k++] =
                new Vector3(Cos(stepAngleSides * j) * radius, 0, Sin(stepAngleSides * j) * radius)
                    - center;

            for (int i = 0; i < verticalResolution; i++)
                for (int j = 0; j < sides; j++)
                    vertices[k++] =
                    new Vector3(Cos(stepAngleSides * j) * radius, stepHeight * i, Sin(stepAngleSides * j) * radius)
                        - center;

            for (int j = 0; j < sides; j++)
                vertices[k++] =
                new Vector3(Cos(stepAngleSides * j) * radius, stepHeight * (verticalResolution - 1), Sin(stepAngleSides * j) * radius)
                    - center;

            vertices[k++] = (Vector3.up * height) - center;
            return vertices;
        }

        private static int[] CreateTriangles(int sides, int verticalResolution)
        {
            int lastIndex = (sides * verticalResolution) + (sides * 2) + 1;

            int[] triangles = new int[(sides * (verticalResolution - 1) * 6) + (sides * 3) * 2];

            int vi = 1;
            int ti = 0;
            for (; vi < sides; ti += 3, vi++)
            {
                triangles[ti] = 0;
                triangles[ti + 1] = vi;
                triangles[ti + 2] = vi + 1;
            }
            triangles[ti] = 0;
            triangles[ti + 1] = vi;
            triangles[ti + 2] = 1;

            ti += 3;
            // vi = 1;
            vi++;

            for (int y = 1; y < verticalResolution; y++, ti += 6, vi++)
            {
                for (int x = 1; x < sides; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + sides;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                    triangles[ti + 5] = vi + sides + 1;
                }
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 4] = vi + sides;
                triangles[ti + 2] = triangles[ti + 3] = vi - sides + 1;
                triangles[ti + 5] = vi + 1;
            }
            vi += sides;

            for (; vi < lastIndex - 1; ti += 3, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = lastIndex;
                triangles[ti + 2] = vi + 1;
            }
            triangles[ti] = vi;
            triangles[ti + 1] = lastIndex;
            triangles[ti + 2] = vi - sides + 1;

            return triangles;
        }
    }
}