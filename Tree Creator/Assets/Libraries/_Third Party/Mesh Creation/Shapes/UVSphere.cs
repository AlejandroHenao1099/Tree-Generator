using UnityEngine;
using static UnityEngine.Mathf;

namespace MeshGenerator
{
    public static class UVSphere
    {
        public static Mesh Create(int horizontalResolution, int verticalResolution, float radius)
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices;

            mesh.name = "UV Sphere";

            vertices = CreateVertices(radius, horizontalResolution, verticalResolution);
            mesh.vertices = vertices;
            mesh.triangles = CreateTriangles(horizontalResolution, verticalResolution, vertices.Length - 1);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Vector3[] CreateVertices(float radius, int horizontalResolution, int verticalResolution)
        {
            var vertices = new Vector3[(horizontalResolution * verticalResolution) + 2];
            vertices[0] = Vector3.down * radius;
            // φ vertical     90  -  -90
            // λ horizontal   0  -  360
            // x = r cos(φ)cos(λ), y = r sin(φ), z = r cos(φ)sin(λ)

            float stepHor = (Mathf.PI * 2) / (float)horizontalResolution;
            float stepVer = Mathf.PI / (float)(verticalResolution + 1f);
            
            for (int i = 1, k = 1; i < verticalResolution + 1; i++)
            {
                for (int j = 0; j < horizontalResolution; j++)
                {
                    Vector3 pos = new Vector3(Cos((-Mathf.PI / 2) + (stepVer * i)) * Cos(stepHor * j),
                                               Sin((-Mathf.PI / 2) + (stepVer * i)),
                                               Cos((-Mathf.PI / 2) + (stepVer * i)) * Sin(stepHor * j));
                    vertices[k++] = pos * (float)radius;
                }
            }
            vertices[vertices.Length - 1] = Vector3.up * (float)radius;
            return vertices;
        }

        private static int[] CreateTriangles(int horizontalResolution, int verticalResolution, int lastIndex)
        {
            int[] triangles = new int[(horizontalResolution * (verticalResolution - 1) * 6) + (horizontalResolution * 3) * 2];

            int vi = 1;
            int ti = 0;
            for (; vi < horizontalResolution; ti += 3, vi++)
            {
                triangles[ti] = 0;
                triangles[ti + 1] = vi;
                triangles[ti + 2] = vi + 1;
            }
            triangles[ti] = 0;
            triangles[ti + 1] = vi;
            triangles[ti + 2] = 1;

            ti += 3;
            vi = 1;

            for (int y = 1; y < verticalResolution; y++, ti += 6, vi++)
            {
                for (int x = 1; x < horizontalResolution; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = triangles[ti + 4] = vi + horizontalResolution;
                    triangles[ti + 2] = triangles[ti + 3] = vi + 1;
                    triangles[ti + 5] = vi + horizontalResolution + 1;
                }
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 4] = vi + horizontalResolution;
                triangles[ti + 2] = triangles[ti + 3] = vi - horizontalResolution + 1;
                triangles[ti + 5] = vi + 1;
            }

            for (; vi < lastIndex - 1; ti += 3, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = lastIndex;
                triangles[ti + 2] = vi + 1;
            }
            triangles[ti] = vi;
            triangles[ti + 1] = lastIndex;
            triangles[ti + 2] = vi - horizontalResolution + 1;

            return triangles;
        }

    }
}