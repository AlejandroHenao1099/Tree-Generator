using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public static class IcoSphere
    {

        private struct TriangleIndices
        {
            public int v1;
            public int v2;
            public int v3;
            public TriangleIndices(int v1, int v2, int v3)
            {
                this.v1 = v1;
                this.v2 = v2;
                this.v3 = v3;
            }
        }

        private struct MeshGeometry
        {
            public List<Vector3> Positions;
            public List<int> TriangleIndices;
            public MeshGeometry(int uno)
            {
                Positions = new List<Vector3>();
                TriangleIndices = new List<int>();
            }
        }

        private static MeshGeometry geometry;
        private static Mesh mesh;
        private static int index;
        private static Dictionary<long, int> middlePointIndexCache;
        // add vertex to mesh, fix position to be on unit sphere, return index
        private static int AddVertex(Vector3 p)
        {
            float length = Mathf.Sqrt(p.x * p.x + p.y * p.y + p.z * p.z);
            geometry.Positions.Add(new Vector3(p.x / length, p.y / length, p.z / length));
            return index++;
        }

        // return index of point in the middle of p1 and p2
        private static int GetMiddlePoint(int p1, int p2)
        {
            // first check if we have it already
            bool firstIsSmaller = p1 < p2;
            long smallerIndex = firstIsSmaller ? p1 : p2;
            long greaterIndex = firstIsSmaller ? p2 : p1;
            long key = (smallerIndex << 32) + greaterIndex;
            int ret;
            if (middlePointIndexCache.TryGetValue(key, out ret))
            {
                return ret;
            }
            // not in cache, calculate it
            Vector3 point1 = geometry.Positions[p1];
            Vector3 point2 = geometry.Positions[p2];
            Vector3 middle = new Vector3(
            (point1.x + point2.x) / 2.0f,
            (point1.y + point2.y) / 2.0f,
            (point1.z + point2.z) / 2.0f);
            // add vertex makes sure point is on unit sphere
            int i = AddVertex(middle);
            // store it, return index
            middlePointIndexCache.Add(key, i);
            return i;
        }

        public static Mesh Create(int recursionLevel)
        {
            geometry = new MeshGeometry(0);
            middlePointIndexCache = new Dictionary<long, int>();
            index = 0;
            // create 12 vertices of a icosahedron
            var t = (1.0f + Mathf.Sqrt(5.0f)) / 2.0f;
            AddVertex(new Vector3(-1, t, 0));
            AddVertex(new Vector3(1, t, 0));
            AddVertex(new Vector3(-1, -t, 0));
            AddVertex(new Vector3(1, -t, 0));
            AddVertex(new Vector3(0, -1, t));
            AddVertex(new Vector3(0, 1, t));
            AddVertex(new Vector3(0, -1, -t));
            AddVertex(new Vector3(0, 1, -t));
            AddVertex(new Vector3(t, 0, -1));
            AddVertex(new Vector3(t, 0, 1));
            AddVertex(new Vector3(-t, 0, -1));
            AddVertex(new Vector3(-t, 0, 1));
            // create 20 triangles of the icosahedron
            var faces = new List<TriangleIndices>();
            // 5 faces around point 0
            faces.Add(new TriangleIndices(0, 11, 5));
            faces.Add(new TriangleIndices(0, 5, 1));
            faces.Add(new TriangleIndices(0, 1, 7));
            faces.Add(new TriangleIndices(0, 7, 10));
            faces.Add(new TriangleIndices(0, 10, 11));
            // 5 adjacent faces
            faces.Add(new TriangleIndices(1, 5, 9));
            faces.Add(new TriangleIndices(5, 11, 4));
            faces.Add(new TriangleIndices(11, 10, 2));
            faces.Add(new TriangleIndices(10, 7, 6));
            faces.Add(new TriangleIndices(7, 1, 8));
            // 5 faces around point 3
            faces.Add(new TriangleIndices(3, 9, 4));
            faces.Add(new TriangleIndices(3, 4, 2));
            faces.Add(new TriangleIndices(3, 2, 6));
            faces.Add(new TriangleIndices(3, 6, 8));
            faces.Add(new TriangleIndices(3, 8, 9));
            // 5 adjacent faces
            faces.Add(new TriangleIndices(4, 9, 5));
            faces.Add(new TriangleIndices(2, 4, 11));
            faces.Add(new TriangleIndices(6, 2, 10));
            faces.Add(new TriangleIndices(8, 6, 7));
            faces.Add(new TriangleIndices(9, 8, 1));
            // refine triangles
            for (int i = 0; i < recursionLevel; i++)
            {
                var faces2 = new List<TriangleIndices>();
                foreach (var tri in faces)
                {
                    // replace triangle by 4 triangles
                    int a = GetMiddlePoint(tri.v1, tri.v2);
                    int b = GetMiddlePoint(tri.v2, tri.v3);
                    int c = GetMiddlePoint(tri.v3, tri.v1);
                    faces2.Add(new TriangleIndices(tri.v1, a, c));
                    faces2.Add(new TriangleIndices(tri.v2, b, a));
                    faces2.Add(new TriangleIndices(tri.v3, c, b));
                    faces2.Add(new TriangleIndices(a, b, c));
                }
                faces = faces2;
            }
            // done, now add triangles to mesh
            foreach (var tri in faces)
            {
                geometry.TriangleIndices.Add(tri.v1);
                geometry.TriangleIndices.Add(tri.v2);
                geometry.TriangleIndices.Add(tri.v3);
            }

            mesh = new Mesh();
            mesh.SetVertices(geometry.Positions);
            mesh.SetTriangles(geometry.TriangleIndices, 0);
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}