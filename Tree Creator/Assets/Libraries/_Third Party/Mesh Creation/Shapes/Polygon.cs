using UnityEngine;

namespace MeshGenerator
{
    public static class Polygon
    {
        public static Mesh Create(int resolution, float width, float height, Vector3 center)
        {
            Mesh mesh = new Mesh();
            mesh.vertices = CreateVertices(resolution, center, width, height);
            mesh.triangles = CreateTriangles(resolution);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Vector3[] CreateVertices(int resolution, Vector3 center, float width, float height)
        {
            Vector3 iterador = Vector3.zero;
            Vector3[] vertices = new Vector3[resolution + 1];
            vertices[0] = center;
            float stepAngle = (2 * Mathf.PI) / resolution;
            for (int i = 1; i <= resolution; i++)
                vertices[i] = new Vector3(Mathf.Cos(stepAngle * i) * width, 0, Mathf.Sin(stepAngle * i) * height);
            return vertices;
        }

        private static int[] CreateTriangles(int resolution)
        {
            int[] triangles = new int[resolution * 3];
            int ti = 0;
            int vi = 1;
            int i = 0;
            for (i = 0, ti = 0, vi = 1; i < resolution - 1; ti += 3, vi++, i++)
            {
                triangles[ti] = 0;
                triangles[ti + 1] = vi + 1;
                triangles[ti + 2] = vi;
            }

            triangles[ti] = 0;
            triangles[ti + 1] = 1;
            triangles[ti + 2] = vi;
            return triangles;
        }
    }
}