using UnityEngine;

namespace MeshGenerator
{
    public static class Cube
    {
        private static int xResolution, yResolution, zResolution;

        private static Vector3[] vertices;
        private static int[] triangles;

        public static Mesh Create(int xSize, int ySize, int zSize, float width, float height, float depth)
        {
            Cube.xResolution = xSize;
            Cube.yResolution = ySize;
            Cube.zResolution = zSize;
            var mesh = new Mesh();
            mesh.name = "Cube";

            CreateVertices(width, height, depth);
            CreateTriangles();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void CreateVertices(float width, float height, float depth)
        {
            int lenghtVertices = (xResolution * yResolution * 2) + (xResolution * zResolution * 2) + (yResolution * zResolution * 2);
            vertices = new Vector3[lenghtVertices];

            var stepWidth = width / (float)(xResolution - 1);
            var stepHeight = height / (float)(yResolution - 1);
            var stepDepth = depth / (float)(zResolution - 1);

            var center = new Vector3(width / 2f, height / 2f, depth / 2f);

            int vi = 0;

            #region XY
            for (int y = 0; y < yResolution; y++)
                for (int x = 0; x < xResolution; x++)
                    vertices[vi++] = new Vector3(x * stepWidth, y * stepHeight, 0) - center;

            for (int y = 0; y < yResolution; y++)
                for (int x = xResolution - 1; x >= 0; x--)
                    vertices[vi++] = new Vector3(x * stepWidth, y * stepHeight, (zResolution - 1) * stepDepth) - center;
            #endregion
            #region ZY
            for (int y = 0; y < yResolution; y++)
                for (int z = 0; z < zResolution; z++)
                    vertices[vi++] = new Vector3(0, y * stepHeight, z * stepDepth) - center;

            for (int y = 0; y < yResolution; y++)
                for (int z = zResolution - 1; z >= 0; z--)
                    vertices[vi++] = new Vector3((xResolution - 1) * stepWidth, y * stepHeight, z * stepDepth) - center;
            #endregion
            #region XZ
            for (int z = 0; z < zResolution; z++)
                for (int x = 0; x < xResolution; x++)
                    vertices[vi++] = new Vector3(x * stepWidth, 0, z * stepDepth) - center;

            for (int z = 0; z < zResolution; z++)
                for (int x = xResolution - 1; x >= 0; x--)
                    vertices[vi++] = new Vector3(x * stepWidth, (yResolution - 1) * stepHeight, z * stepDepth) - center;
            #endregion
        }

        private static void CreateTriangles()
        {
            int lenghtTriangles = ((xResolution - 1) * (yResolution - 1) * 2) +
                    ((xResolution - 1) * (zResolution - 1) * 2) + ((yResolution - 1) * (zResolution - 1) * 2);
            triangles = new int[lenghtTriangles * 6];

            var horizontalResolution = xResolution;
            var verticalResolution = yResolution;
            var inverse = false;
            for (int n = 0, vi = 0, ti = 0; n < 6; n++, vi += horizontalResolution)
            {
                if (n < 2)
                {
                    horizontalResolution = xResolution;
                    verticalResolution = yResolution;
                    inverse = false;
                }
                else if (n < 4)
                {
                    horizontalResolution = zResolution;
                    verticalResolution = yResolution;
                    inverse = true;
                }
                else
                {
                    horizontalResolution = xResolution;
                    verticalResolution = zResolution;
                    inverse = true;
                }
                for (int i = 0; i < verticalResolution - 1; i++, vi++)
                    for (int j = 0; j < horizontalResolution - 1; j++, ti += 6, vi++)
                        SetQuad(ti, vi, horizontalResolution, inverse);
            }
        }

        private static void SetQuad(int ti, int vi, int horizontalResolution, bool inverse = false)
        {
            if (inverse == false)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + horizontalResolution;
                triangles[ti + 2] = vi + 1;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + horizontalResolution;
                triangles[ti + 5] = vi + horizontalResolution + 1;
            }
            else
            {
                triangles[ti] = vi;
                triangles[ti + 1] = vi + 1;
                triangles[ti + 2] = vi + horizontalResolution;
                triangles[ti + 3] = vi + 1;
                triangles[ti + 4] = vi + horizontalResolution + 1;
                triangles[ti + 5] = vi + horizontalResolution;
            }
        }

    }
}