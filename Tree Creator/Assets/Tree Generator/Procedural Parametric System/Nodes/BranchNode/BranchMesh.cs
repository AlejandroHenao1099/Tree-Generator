using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class BranchNode
    {
        private Mesh mesh;
        // private MeshData meshData;
        // private List<Vector3> vertices, tangents;
        private List<Vector3> vertices;
        // private List<Vector2> uv;
        private List<int> triangles;
        private NormalizeMeshData meshData;
        // private DynamicSpline dynamicSpline;
        private int resolutionVertical, resolutionHorizontal;


        public void InitializeMesh()
        {
            vertices = new List<Vector3>();
            triangles = new List<int>();
            // tangents = new List<Vector3>();
            // uv = new List<Vector2>();
            mesh = new Mesh();
            mesh.name = "Trunk";
            meshData = new NormalizeMeshData();
            meshData.vertices = new List<Vector3>();
            meshData.normals = new List<Vector3>();
            // meshData = new MeshData(mesh, 3);
            this.resolutionVertical = branchData.ResolutionVertical;
            this.resolutionHorizontal = branchData.ResolutionHorizontal;

            UpdateQuantityVertices(0, this.resolutionVertical, 0, this.resolutionHorizontal);
            UpdateQuantityTriangles(0, this.resolutionVertical, 0, this.resolutionHorizontal);
            UpdateTriangleRing(0, this.resolutionVertical, this.resolutionVertical, this.resolutionHorizontal);
        }

        private void UpdateQuantityVertices(int prevVer, int newVer, int prevHor, int newHor)
        {
            int prevQuantityVertices = GetDesiredVertices(prevVer, prevHor);
            int newQuantityVertices = GetDesiredVertices(newVer, newHor);

            if (prevQuantityVertices < newQuantityVertices)
            {
                for (int i = prevQuantityVertices; i < newQuantityVertices; i++)
                {
                    vertices.Add(Vector3.zero);
                    meshData.vertices.Add(Vector3.zero);
                    meshData.normals.Add(Vector3.zero);
                }
            }
            else if (prevQuantityVertices > newQuantityVertices)
            {
                for (int i = prevQuantityVertices; i != newQuantityVertices; i--)
                {
                    int indexToRemove = vertices.Count - 1;
                    vertices.RemoveAt(indexToRemove);
                    meshData.vertices.RemoveAt(indexToRemove);
                    meshData.normals.RemoveAt(indexToRemove);
                }
            }
        }

        private void UpdateQuantityTriangles(int prevVer, int newVer, int prevHor, int newHor)
        {
            int prevQuantityTris = GetDesiredTriangles(prevVer, prevHor);
            int newQuantityTris = GetDesiredTriangles(newVer, newHor);

            if (prevQuantityTris < newQuantityTris)
                for (int i = prevQuantityTris; i < newQuantityTris; i++)
                    triangles.Add(-1);
            else if (prevQuantityTris > newQuantityTris)
            {
                for (int i = prevQuantityTris; i != newQuantityTris; i--)
                    triangles.RemoveAt(triangles.Count - 1);
            }
        }

        private void UpdateTriangleRing(int from, int to, int ver, int hor)
        {
            int vi = GetDesiredVertexIndex(from, hor, ver);
            int ti = GetDesiredTriangleIndex(from, hor, ver);
            for (int i = from; i < to; i++, vi++, ti += 6)
            {
                for (int j = 0; j < hor - 1; j++, vi++, ti += 6)
                {
                    triangles[ti] = vi;
                    triangles[ti + 1] = vi + 1;
                    triangles[ti + 2] = vi + hor;
                    triangles[ti + 3] = vi + 1;
                    triangles[ti + 4] = vi + hor + 1;
                    triangles[ti + 5] = vi + hor;
                }
                triangles[ti] = vi;
                triangles[ti + 1] = vi - (hor - 1);
                triangles[ti + 2] = vi + hor;
                triangles[ti + 3] = vi - (hor - 1);
                triangles[ti + 4] = vi + 1;
                triangles[ti + 5] = vi + hor;
            }
        }

        private void UpdateBranchMesh()
        {
            if (branchData.ShapeCurve == ShapeCurve.Default)
                UpdateMesh(in mainSpline);
            else
                UpdateMesh(in secondarySpline);
        }

        private void UpdateMesh(in DynamicSpline dynamicSpline)
        {
            UpdateResolutionMesh();
            var walkerSpline = new WalkerSpline(dynamicSpline.GetDerivative(0), Vector3.up);
            float stepRes = 1f / (float)(resolutionVertical);
            var currentDirection = Vector3.zero;
            for (int i = 0, n = 0; i < resolutionVertical + 1; i++)
            {
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                float currentRadius = GetRadiusAt(i * stepRes);

                UpdateRingMesh(n, currentRadius, in walkerSpline);
                n += resolutionHorizontal;
                currentDirection = dynamicSpline.GetDerivative(i * stepRes);
                walkerSpline.UpdateDirection(currentDirection);
            }
            mesh.Clear(true);
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
        }

        private void UpdateRingMesh(int n, float currentRadius, in WalkerSpline walkerSpline)
        {
            float stepAngle = 360f / resolutionHorizontal;
            float lobeDepth = treeData.LobeDepth * branchData.ScalerLobeDepth;
            int lobes = treeData.Lobes;
            float offsetLobes = n * branchData.OffsetLobes;
            // float scale = trunkData.nScale + trunkData.nScaleV;
            for (int j = 0; j < resolutionHorizontal; j++)
            {
                float angle = stepAngle * j;
                // float lobeZ = 1f + lobeDepth * Mathf.Sin(lobes * angle) * scale;
                float lobe_Z = 1f + lobeDepth * Mathf.Sin((float)lobes * ((Mathf.Deg2Rad * angle) + offsetLobes));
                var vertex = walkerSpline.GetCylindricalPoint(angle, currentRadius);
                meshData.vertices[n] = vertex;
                vertex = walkerSpline.GetCylindricalPoint(angle, currentRadius * lobe_Z);
                vertices[n++] = vertex;
            }
            CalculateNormalsRing(n - resolutionHorizontal);
        }

        private void CalculateNormalsRing(int n)
        {
            bool evenRing = (resolutionHorizontal & 1) == 0;

            for (int j = 0; j < resolutionHorizontal; j++)
            {
                int opuesto = -1;
                Vector3 oppositeVertex = Vector3.zero;
                if (evenRing == true)
                {
                    opuesto = j + resolutionHorizontal / 2;
                    opuesto %= resolutionHorizontal;
                    oppositeVertex = meshData.vertices[n + opuesto];
                }
                else
                {
                    opuesto = j + Mathf.CeilToInt((float)resolutionHorizontal / 2f);
                    int oppositePrev = opuesto - 1;
                    opuesto %= resolutionHorizontal;
                    oppositePrev %= resolutionHorizontal;
                    oppositeVertex = Vector3.Lerp(meshData.vertices[n + opuesto],
                        meshData.vertices[n + oppositePrev], 0.5f);
                }
                meshData.normals[n + j] = (meshData.vertices[n + j] - oppositeVertex).normalized;
            }
        }

        private void UpdateResolutionMesh()
        {
            int prevVer = resolutionVertical;
            int prevHor = resolutionHorizontal;
            int newVer = branchData.ResolutionVertical;
            int newHor = branchData.ResolutionHorizontal;
            resolutionVertical = newVer;
            resolutionHorizontal = newHor;
            UpdateQuantityVertices(prevVer, newVer, prevHor, newHor);
            UpdateQuantityTriangles(prevVer, newVer, prevHor, newHor);

            if (newVer > prevVer)
                UpdateTriangleRing(prevVer, newVer, newVer, newHor);
            else if(newHor != prevHor)
                UpdateTriangleRing(0, newVer, newVer, newHor);
        }   

        private int GetDesiredVertices(int ver, int hor) => (ver + 1) * hor;
        private int GetDesiredTriangles(int ver, int hor) => ver * hor * 6;
        
        private int GetDesiredVertexIndex(int ringIndex, int hor, int ver)
        {
            if (ringIndex >= ver || ringIndex < 0 || ver <= 0)
                throw new System.Exception("Inconsistentes Rings");
            return ringIndex * hor;
        }

        private int GetDesiredTriangleIndex(int ringIndex, int hor, int ver)
        {
            if (ringIndex >= ver || ringIndex < 0 || ver <= 0)
                throw new System.Exception("Inconsistentes Rings");
            return ringIndex * hor * 6;
        }
    }
}