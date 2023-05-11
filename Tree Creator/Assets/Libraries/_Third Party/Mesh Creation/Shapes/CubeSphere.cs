using UnityEngine;

namespace MeshGenerator
{
    public static class CubeSphere
    {
        private static int resolution;
        private static float radius;


        private static Vector3[] vertices;
        private static Vector3[] normals;
        private static int[] triangles;

        public static Mesh Create(int resolution, float radius)
        {
            CubeSphere.radius = radius;
            CubeSphere.resolution = resolution;
            var mesh = new Mesh();
            mesh.name = "Cube Sphere";

            CreateVertices();
            CreateTriangles();

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void CreateVertices()
        {
            int resMin = resolution % 2 == 0 ? 2 : 3;
            int lenghtRing = resolution * 2 + (resolution - 2) * 2;
            int sizeVertices = lenghtRing * (resolution - 2) + (resolution * resolution * 2);
            vertices = new Vector3[sizeVertices];
            int vi = 0;
            if (resMin == 3)
                SetVertex(vi++, Mathf.CeilToInt((float)resolution / 2f) - 1, 0, Mathf.CeilToInt((float)resolution / 2f) - 1);

            Vector3Int currCorner = new Vector3Int((resolution / 2) - 1, 0, (resolution / 2) - 1);

            for (int currRes = resMin; currRes <= resolution; currRes += 2)
            {
                for (int x = 0; x < currRes; x++)
                    SetVertex(vi++, currCorner.x + x, 0, currCorner.z);
                for (int z = 1; z < currRes; z++)
                    SetVertex(vi++, currCorner.x + currRes - 1, 0, currCorner.z + z);

                for (int x = currRes - 2; x >= 0; x--)
                    SetVertex(vi++, currCorner.x + x, 0, currCorner.z + currRes - 1);
                for (int z = currRes - 2; z > 0; z--)
                    SetVertex(vi++, currCorner.x, 0, currCorner.z + z);

                currCorner = new Vector3Int(currCorner.x - 1, 0, currCorner.z - 1);
            }

            for (int y = 1; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                    SetVertex(vi++, x, y, 0);
                for (int z = 1; z < resolution; z++)
                    SetVertex(vi++, resolution - 1, y, z);

                for (int x = resolution - 2; x >= 0; x--)
                    SetVertex(vi++, x, y, resolution - 1);
                for (int z = resolution - 2; z > 0; z--)
                    SetVertex(vi++, 0, y, z);
            }

            currCorner = new Vector3Int(1, resolution - 1, 1);
            for (int currRes = resolution - 2, y = resolution - 1; currRes >= resMin; currRes -= 2)
            {
                for (int x = 0; x < currRes; x++)
                    SetVertex(vi++, currCorner.x + x, y, currCorner.z);
                for (int z = 1; z < currRes; z++)
                    SetVertex(vi++, currCorner.x + currRes - 1, y, currCorner.z + z);

                for (int x = currRes - 2; x >= 0; x--)
                    SetVertex(vi++, currCorner.x + x, y, currCorner.z + currRes - 1);
                for (int z = currRes - 2; z > 0; z--)
                    SetVertex(vi++, currCorner.x, y, currCorner.z + z);

                currCorner = new Vector3Int(currCorner.x + 1, y, currCorner.z + 1);
            }
            if (resMin == 3)
                SetVertex(vi++, Mathf.CeilToInt((float)resolution / 2f) - 1, resolution - 1, Mathf.CeilToInt((float)resolution / 2f) - 1);
        }

        private static void SetVertex(int i, int x, int y, int z)
        {
            // vertices[i] = new Vector3(x, y, z);
            float stepRes = 2f / (float)(resolution - 1);
            Vector3 v = new Vector3(x, y, z) * stepRes - Vector3.one;

            float x2 = v.x * v.x;
            float y2 = v.y * v.y;
            float z2 = v.z * v.z;
            Vector3 s;
            s.x = v.x * Mathf.Sqrt(1f - y2 / 2f - z2 / 2f + y2 * z2 / 3f);
            s.y = v.y * Mathf.Sqrt(1f - x2 / 2f - z2 / 2f + x2 * z2 / 3f);
            s.z = v.z * Mathf.Sqrt(1f - x2 / 2f - y2 / 2f + x2 * y2 / 3f);

            vertices[i] = s * radius;
            // cubeUV[i] = new Color32((byte)x, (byte)y, (byte)z, 0);
        }

        private static void CreateTriangles()
        {
            int quads = resolution * resolution * 6;
            triangles = new int[quads * 6];
            int ti = 0, vi = 0;
            CreateBottomFace(ref vi, ref ti);
            CreateLateralFaces(ref vi, ref ti);
            CreateTopFace(vi, ti);
        }

        private static void CreateLateralFaces(ref int vi, ref int ti)
        {
            int lenghtRing = resolution * 2 + (resolution - 2) * 2;

            for (int ver = 0; ver < resolution - 1; ver++)
            {
                for (int hor = 0; hor < ((resolution - 1) * 4) - 1; hor++)
                {
                    ti = SetQuad(ti, vi, vi + 1, vi + lenghtRing, vi + lenghtRing + 1);
                    vi++;
                }
                ti = SetQuad(ti, vi, vi - (lenghtRing - 1), vi + lenghtRing, vi + 1);
                vi++;
            }
        }

        private static void CreateBottomFace(ref int vi, ref int ti)
        {
            int currCorner = vi;
            int prevCorner = vi;
            int currResRing = 2;
            int prevResRing = 2;

            int resMin = resolution % 2 == 0 ? 2 : 3;
            if (resMin == 2)
            {
                ti = SetQuad(ti, vi, vi + 3, vi + 1, vi + 2);
                vi += 4;
            }
            else
            {
                vi++;
                ti = SetQuad(ti, vi, vi + 7, vi + 1, 0);
                vi++;
                ti = SetQuad(ti, vi, 0, vi + 1, vi + 2);
                vi += 2;
                ti = SetQuad(ti, 0, vi + 2, vi, vi + 1);
                vi += 2;
                ti = SetQuad(ti, vi + 2, vi + 1, 0, vi);
                vi += 3;
                currResRing = 3;
                prevResRing = 3;
                prevCorner = vi - 8;
            }
            int lenghtCurrRing = currResRing * 2 + (currResRing - 2) * 2;
            int lenghtPrevRing = currResRing * 2 + (currResRing - 2) * 2;

            currCorner = vi;
            currResRing += 2;
            lenghtCurrRing = currResRing * 2 + (currResRing - 2) * 2;

            for (int k = 0; currResRing <= resolution; k = 0)
            {
                ti = SetQuad(ti, vi, vi + lenghtCurrRing - 1, vi + 1, prevCorner);
                vi++;
                for (int j = 0; j < prevResRing - 1; j++)
                {
                    ti = SetQuad(ti, vi, prevCorner + k, vi + 1, prevCorner + k + 1);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, vi, prevCorner + k, vi + 1, vi + 2);
                vi += 2;

                for (int j = 0; j < prevResRing - 1; j++)
                {
                    ti = SetQuad(ti, prevCorner + k, prevCorner + k + 1, vi, vi + 1);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, prevCorner + k, vi + 2, vi, vi + 1);
                vi += 2;

                for (int j = 0; j < prevResRing - 1; j++)
                {
                    ti = SetQuad(ti, prevCorner + k + 1, vi + 1, prevCorner + k, vi);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, vi + 2, vi + 1, prevCorner + k, vi);
                vi += 2;

                for (int j = 0; j < prevResRing - 1; j++)
                {
                    int next = k >= lenghtPrevRing - 1 ? 0 : k + 1;
                    ti = SetQuad(ti, vi + 1, vi, prevCorner + next, prevCorner + k);
                    k++;
                    vi++;
                }

                prevResRing = currResRing;
                currResRing += 2;
                vi++;
                prevCorner = currCorner;
                currCorner = vi;
                lenghtPrevRing = lenghtCurrRing;
                lenghtCurrRing = currResRing * 2 + (currResRing - 2) * 2;
            }
            vi = prevCorner;
        }

        private static void CreateTopFace(int vi, int ti)
        {
            int currResRing = resolution;
            int nextResRing = resolution - 2;
            int lenghtCurrRing = currResRing * 2 + (currResRing - 2) * 2;
            int lenghtNextRing = nextResRing * 2 + (nextResRing - 2) * 2;
            int currCorner = vi;
            int nextCorner = vi + lenghtCurrRing;
            int resMin = resolution % 2 == 0 ? 2 : 3;

            for (int k = 0; currResRing >= 4; k = 0)
            {
                ti = SetQuad(ti, vi, vi + 1, vi + lenghtCurrRing - 1, nextCorner);
                vi++;

                for (int j = 0; j < nextResRing - 1; j++)
                {
                    ti = SetQuad(ti, vi, vi + 1, nextCorner + k, nextCorner + k + 1);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, vi, vi + 1, nextCorner + k, vi + 2);
                vi += 2;

                for (int j = 0; j < nextResRing - 1; j++)
                {
                    ti = SetQuad(ti, nextCorner + k, vi, nextCorner + k + 1, vi + 1);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, nextCorner + k, vi, vi + 2, vi + 1);
                vi += 2;

                for (int j = 0; j < nextResRing - 1; j++)
                {
                    ti = SetQuad(ti, nextCorner + k + 1, nextCorner + k, vi + 1, vi);
                    k++;
                    vi++;
                }

                ti = SetQuad(ti, vi + 2, nextCorner + k, vi + 1, vi);
                vi += 2;

                for (int j = 0; j < nextResRing - 1; j++)
                {
                    int next = k >= lenghtNextRing - 1 ? 0 : k + 1;
                    ti = SetQuad(ti, vi + 1, nextCorner + next, vi, nextCorner + k);
                    k++;
                    vi++;
                }

                nextResRing -= 2;
                currResRing -= 2;
                vi++;
                nextCorner = nextCorner + lenghtNextRing;
                currCorner = vi;
                lenghtNextRing = nextResRing * 2 + (nextResRing - 2) * 2;
                lenghtCurrRing = currResRing * 2 + (currResRing - 2) * 2;
            }
            if (resMin == 2)
                ti = SetQuad(ti, vi, vi + 1, vi + 3, vi + 2);
            else
            {
                int last = vertices.Length - 1;
                ti = SetQuad(ti, vi, vi + 1, vi + 7, last);
                vi++;
                ti = SetQuad(ti, vi, vi + 1, last, vi + 2);
                vi += 2;
                ti = SetQuad(ti, last, vi, vi + 2, vi + 1);
                vi += 2;
                ti = SetQuad(ti, vi + 2, last, vi + 1, vi);
            }
        }

        private static int SetQuad(int i, int v00, int v10, int v01, int v11)
        {
            triangles[i] = v00;
            triangles[i + 1] = triangles[i + 4] = v01;
            triangles[i + 2] = triangles[i + 3] = v10;
            triangles[i + 5] = v11;
            return i + 6;
        }
    }
}