using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class TrunkNode
    {
        private Mesh mesh;
        // private MeshData meshData;
        // private List<Vector3> vertices, tangents;
        private List<Vector3> vertices;
        // private List<Vector3> normals;
        // private List<Vector2> uv;
        private List<int> triangles;
        private NormalizeMeshData meshData;
        // private int GetResolutionVertical(), GetResolutionHorizontal();

        public void InitializeMesh()
        {
            vertices = new List<Vector3>();
            // normals = new List<Vector3>();
            triangles = new List<int>();
            // tangents = new List<Vector3>();
            // uv = new List<Vector2>();
            mesh = new Mesh();
            mesh.name = "Trunk";
            meshData = new NormalizeMeshData();
            meshData.vertices = new List<Vector3>();
            meshData.normals = new List<Vector3>();
            // meshData = new MeshData(mesh, 3);
            // this.resolutionVertical = GetResolutionVertical();
            // this.resolutionHorizontal = GetResolutionHorizontal();

            for (int i = 0; i < (GetResolutionVertical() + 1) * GetResolutionHorizontal(); i++)
            {
                vertices.Add(Vector3.zero);
                // normals.Add(Vector3.zero);
                meshData.vertices.Add(Vector3.zero);
                meshData.normals.Add(Vector3.zero);
            }

            CreateTriangles();
        }

        private void UpdateBranchMesh()
        {
            if (trunkData.ShapeCurve == ShapeCurve.Default)
                UpdateMesh(in mainSpline);
            else
                UpdateMesh(in secondarySpline);
        }

        private void UpdateMesh(in DynamicSpline dynamicSpline)
        {
            int hor = GetResolutionHorizontal();
            int ver = GetResolutionVertical();
            trunkData = treeData.GetTrunkData();
            var walkerSpline = new WalkerSpline(Vector3.up, Vector3.forward);
            float stepRes = 1f / (float)(ver - 1);
            float stepAngle = 360f / hor;

            var currentDirection = Vector3.zero;
            for (int i = 0, n = 0; i < ver; i++)
            {
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                float currentRadius = GetRadiusAt(i * stepRes);

                UpdateRingMesh(n, currentRadius, in walkerSpline);
                n += hor;
                currentDirection = dynamicSpline.GetDerivative(i * stepRes);
                walkerSpline.UpdateDirection(currentDirection);
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            // mesh.SetNormals(normals);
            mesh.RecalculateNormals();
        }

        private void UpdateRingMesh(int n, float currentRadius, in WalkerSpline walkerSpline)
        {
            int hor = GetResolutionHorizontal();
            float stepAngle = 360f / hor;
            float lobeDepth = treeData.LobeDepth;
            int lobes = treeData.Lobes;
            float offsetLobes = n * trunkData.OffsetLobes;
            // float scale = trunkData.nScale + trunkData.nScaleV;
            for (int j = 0; j < hor; j++)
            {
                float angle = stepAngle * j;
                // float lobeZ = 1f + lobeDepth * Mathf.Sin(lobes * angle) * scale;
                float lobe_Z = 1f + lobeDepth * Mathf.Sin((float)lobes * ((Mathf.Deg2Rad * angle) + offsetLobes));
                var vertex = walkerSpline.GetCylindricalPoint(angle, currentRadius);
                meshData.vertices[n] = vertex;
                vertex = walkerSpline.GetCylindricalPoint(angle, currentRadius * lobe_Z);
                vertices[n++] = vertex;
            }
            CalculateNormalsRing(n - hor);
        }

        private void CalculateNormalsRing(int n)
        {
            int hor = GetResolutionHorizontal();
            bool evenRing = (hor & 1) == 0;
            for (int j = 0; j < hor; j++)
            {
                int opuesto = -1;
                Vector3 oppositeVertex = Vector3.zero;
                if (evenRing == true)
                {
                    opuesto = j + hor / 2;
                    opuesto %= hor;
                    oppositeVertex = meshData.vertices[n + opuesto];
                }
                else
                {
                    opuesto = j + hor / 2;
                    opuesto %= hor;
                    int oppositePrev = opuesto - 1;
                    oppositePrev %= hor;

                    oppositeVertex = Vector3.Lerp(meshData.vertices[n + opuesto], 
                        meshData.vertices[n + oppositePrev], 0.5f);
                }
                // normals[n + j] = (vertices[n + j] - oppositeVertex).normalized;
                meshData.normals[n + j] = (meshData.vertices[n + j] - oppositeVertex).normalized;
            }
        }

        private void CreateTriangles()
        {
            int hor = GetResolutionHorizontal();
            int ver = GetResolutionVertical();
            int vi = 0;
            for (int i = 0; i < ver - 1; i++, vi++)
            {
                for (int j = 0; j < hor - 1; j++, vi++)
                    SetQuad(vi, hor);

                SetTriangle(vi, vi - (hor - 1), vi + hor);
                SetTriangle(vi - (hor - 1), vi + 1, vi + hor);
            }
        }

        private void SetQuad(int currentIndex, int resolution)
        {
            SetTriangle(currentIndex, currentIndex + 1, currentIndex + resolution);
            SetTriangle(currentIndex + 1, currentIndex + resolution + 1, currentIndex + resolution);
        }

        private void SetTriangle(int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
    }
}