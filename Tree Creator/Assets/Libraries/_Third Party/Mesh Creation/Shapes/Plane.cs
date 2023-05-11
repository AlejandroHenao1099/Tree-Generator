using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public static class Plane
    {
        public static Mesh Create(int horizontalResolution, int verticalResolution, float width, float height)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Plane";
            mesh.vertices = CreateVertices(horizontalResolution, verticalResolution, width, height);
            mesh.triangles = CreateTriangles(horizontalResolution, verticalResolution);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static Vector3[] CreateVertices(int horizontalResolution, int verticalResolution, float width, float height)
        {
            var vertices = new Vector3[horizontalResolution * verticalResolution];

            var widthStep = width / (float)(horizontalResolution - 1);
            var heightStep = height / (float)(verticalResolution - 1);
            var middle = new Vector3(width / 2f, 0, height / 2f);

            for (int i = 0, n = 0; i < verticalResolution; i++)
                for (int j = 0; j < horizontalResolution; j++)
                    vertices[n++] = new Vector3(widthStep * j, 0, heightStep * i) - middle;

            return vertices;
        }

        private static int[] CreateTriangles(int horizontalResolution, int verticalResolution)
        {
            var triangles = new int[(horizontalResolution - 1) * (verticalResolution - 1) * 6];

            for (int i = 0, ti = 0, vi = 0; i < verticalResolution - 1; i++, vi++)
            {
                for (int j = 0; j < horizontalResolution - 1; j++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + horizontalResolution;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + horizontalResolution;
                    triangles[ti + 5] = vi + horizontalResolution + 1;
                }
            }
            return triangles;
        }
    }
}