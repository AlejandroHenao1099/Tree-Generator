using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public partial class Plane
    {

        private void UpdateUnityMeshData()
        {
            mesh.Clear(true);
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
        }

        private void UpdateResolutionMesh()
        {
            UpdateSizeVertices(GetDesiredVertices());
            UpdateSizeTriangles(GetDesiredTriangles());
            // UpdateSizeVertex();
            // UpdateVertices();
        }

        private void UpdateVerticalFaces(int from, int to)
        {
            int vi = GetVertexIndex(from);
            int ti = GetTriangleIndex(from);
            UpdateTriangles(from, to, vi, ti);
        }

        private void UpdateHorizontalFaces()
        {
            UpdateTriangles(0, VerticalFaces, 0, 0);
        }

        private void UpdateTriangles(int from, int to, int vi, int ti)
        {
            for (int i = from; i < to; i++, vi++)
            {
                for (int j = 0; j < HorizontalFaces; j++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + horizontalVertices;
                    triangles[ti + 2] = vi + 1;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + horizontalVertices;
                    triangles[ti + 5] = vi + horizontalVertices + 1;
                }
            }
        }
        
    }
}