using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public abstract class MeshData
    {
        protected List<Vector3> vertices;
        protected List<Vector3> normals;
        protected List<int> triangles;

        public List<Vector3> Vertices { get => vertices; }
        public List<Vector3> Normals { get => normals; }
        public List<int> Triangles { get => triangles; }

        public MeshData()
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            triangles = new List<int>();
        }

        public void UpdateSizeVertices(int desiredVertices)
        {
            UpdateSizeList<Vector3>(vertices, desiredVertices, Vector3.zero);
        }

        public void UpdateSizeNormals(int desiredNormals)
        {
            UpdateSizeList<Vector3>(normals, desiredNormals, Vector3.zero);
        }

        public void UpdateSizeTriangles(int desiredTriangles)
        {
            UpdateSizeList<int>(triangles, desiredTriangles, -1);
        }

        protected void UpdateSizeList<T>(List<T> meshList, int desiredSize, T newData)
        {
            int currentSize = meshList.Count;
            if (desiredSize > currentSize)
                for (int i = currentSize; i < desiredSize; i++)
                    meshList.Add(newData);
            else if (desiredSize < currentSize)
                for (int i = currentSize; i != desiredSize; i--)
                    meshList.RemoveAt(meshList.Count - 1);
        }
    }
}