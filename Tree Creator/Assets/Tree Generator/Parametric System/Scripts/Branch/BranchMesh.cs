using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public class BranchMesh
    {
        private Mesh mesh;
        private MeshData meshData;
        private List<Vector3> vertices, tangents;
        private List<Vector2> uv;
        private List<int> triangles;
        private DynamicSpline dynamicSpline;
        private int resolutionVertical, resolutionHorizontal;
        private TreeData treeData;
        private BranchData branchData;
        private Branch branch;
        private int branchLevel;

        public Mesh GetMesh() => mesh;


        public BranchMesh(Branch branch, TreeData treeData, int branchLevel)
        {
            this.branch = branch;
            this.treeData = treeData;
            this.branchLevel = branchLevel;
            branchData = treeData.GetBranchData(branchLevel);

            vertices = new List<Vector3>();
            triangles = new List<int>();
            // tangents = new List<Vector3>();
            // uv = new List<Vector2>();
            mesh = new Mesh();
            mesh.name = "Spline";
            // meshData = new MeshData(mesh, 3);
            this.resolutionVertical = branchData.ResolutionVertical;
            this.resolutionHorizontal = branchData.ResolutionHorizontal;

            for (int i = 0; i < (resolutionVertical + 1) * resolutionHorizontal; i++)
                vertices.Add(Vector3.zero);

            CreateTriangles();
        }

        public void UpdateSpline(DynamicSpline newSpline) => dynamicSpline = newSpline;

        public void UpdateMesh()
        {
            branchData = treeData.GetBranchData(branchLevel);

            var orientation = branch.GetOrientationBranch();
            var walkerSpline = new WalkerSpline(orientation.GetForward(), orientation.GetUp());
            // var origin = orientation.GetPosition();
            float stepRes = 1f / (float)(resolutionVertical - 1);

            var currentDirection = Vector3.zero;
            for (int i = 0, n = 0; i < resolutionVertical; i++)
            {
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                float currentRadius = GetRadiusAt(i * stepRes);

                UpdateRingMesh(n, currentRadius, in walkerSpline);
                n += resolutionHorizontal;
                currentDirection = dynamicSpline.GetDerivative(i * stepRes);
                walkerSpline.UpdateDirection(currentDirection);
            }
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
                float lobe_Z = 1f + lobeDepth * Mathf.Sin((float)lobes * (angle * Mathf.Deg2Rad + offsetLobes));
                var vertex = walkerSpline.GetCylindricalPoint(angle, currentRadius * lobe_Z);
                vertices[n++] = vertex;
            }
        }

        public Vector3 GetCylindricalCoordinates(float normalizePosition, float angle, float radius = 1f)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            var walkerSpline = new WalkerSpline(Vector3.forward, Vector3.up);
            if (normalizePosition.Equals(1))
                return walkerSpline.GetCylindricalPoint(angle, radius);

            int quantitySteps = Mathf.RoundToInt(Mathf.Lerp(0, resolutionVertical - 1, normalizePosition));
            float stepRes = normalizePosition / (float)(quantitySteps);

            for (int i = 0; i < quantitySteps; i++)
            {
                var next = i + 1 >= quantitySteps ? i : i + 1;
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                var currentDirection = (dynamicSpline.GetPoint(i * stepRes)
                        - dynamicSpline.GetPoint(next * stepRes)).normalized;
                if (i + 1 < quantitySteps)
                    walkerSpline.UpdateDirection(currentDirection);
            }
            return walkerSpline.GetCylindricalPoint(angle, radius);
        }

        private void CreateTriangles()
        {
            int vi = 0;
            for (int i = 0; i < resolutionVertical - 1; i++, vi++)
            {
                for (int j = 0; j < resolutionHorizontal - 1; j++, vi++)
                    SetQuad(vi, resolutionHorizontal);

                SetTriangle(vi, vi - (resolutionHorizontal - 1), vi + resolutionHorizontal);
                SetTriangle(vi - (resolutionHorizontal - 1), vi + 1, vi + resolutionHorizontal);
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

        private float GetRadiusAt(float z)
        {
            z = Mathf.Clamp01(z);
            float unit_taper = GetUnitTaper();
            float taper = branchData.NTaper;
            float radiusStem = branch.GetRadiusBase();
            float taperZ = radiusStem * (1f - unit_taper * z);

            float radiusZ = 0f;
            if (taper >= 0 && taper < 1)
                radiusZ = taperZ;
            else if (taper >= 1 && taper <= 3)
            {
                float z_2 = (1f - z) * branch.GetLenght();
                float depth = 0;
                if (taper < 2 || z_2 < taperZ)
                    depth = 1;
                else
                    depth = taper - 2;

                float z_3 = 0f;
                if (taper < 2)
                    z_3 = z_2;
                else
                    z_3 = z_2 - 2f * taperZ * (int)(z_2 / (2 * taperZ) + 0.5f);

                if (taper < 2 && z_3 >= taperZ)
                    radiusZ = taperZ;
                else
                    radiusZ = (1f - depth) * taperZ + depth *
                        Mathf.Sqrt(Mathf.Abs((taperZ * taperZ) - (z_3 - taperZ) * (z_3 - taperZ)));
            }
            return radiusZ;
            // float y = 1f - 8 * z;
            // y = Mathf.Max(0, y);
            // float flareZ = treeData.flare * (Mathf.Pow(100f, y) - 1f) / 100 + 1;
            // return radiusZ * flareZ;
        }

        private float GetUnitTaper()
        {
            if (branchData.NTaper >= 0f && branchData.NTaper < 1f)
                return branchData.NTaper;
            if (branchData.NTaper >= 1f && branchData.NTaper < 2f)
                return 2f - branchData.NTaper;
            // if (trunkData.NTaper >= 2f && trunkData.NTaper < 3f)
            return 0f;
        }

        public static Mesh CreateMesh(in DynamicSpline dynamicSpline, int resolutionVertical, int resolutionHorizontal,
            Vector3 initialUp)
        {
            Mesh mesh = new Mesh();
            mesh.name = "Spline";

            var vertices = new Vector3[(resolutionVertical + 1) * resolutionHorizontal];
            var triangles = new List<int>();

            var walkerSpline = new WalkerSpline(dynamicSpline.GetDerivative(0), initialUp);
            // var walkerSpline = new WalkerSpline(Vector3.forward, Vector3.up);
            float stepRes = 1f / (float)(resolutionVertical - 1);
            float stepAngle = 360f / resolutionHorizontal;

            for (int i = 0, n = 0; i < resolutionVertical; i++)
            {
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                for (int j = 0; j < resolutionHorizontal; j++)
                {
                    float currentAngle = stepAngle * j;
                    var vertex = walkerSpline.GetCylindricalPoint(currentAngle, 1f);
                    vertices[n++] = vertex;
                }
                var currentDirection = dynamicSpline.GetDerivative(i * stepRes).normalized;
                walkerSpline.UpdateDirection(currentDirection);
            }
            SetTriangles(triangles, resolutionVertical, resolutionHorizontal);
            mesh.vertices = vertices;
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            return mesh;
        }

        private static void SetTriangles(List<int> triangles, int resolutionVertical, int resolutionHorizontal)
        {
            int vi = 0;
            for (int i = 0; i < resolutionVertical - 1; i++, vi++)
            {
                for (int j = 0; j < resolutionHorizontal - 1; j++, vi++)
                    SetQuad(triangles, vi, resolutionHorizontal);

                SetTriangle(triangles, vi, vi - (resolutionHorizontal - 1), vi + resolutionHorizontal);
                SetTriangle(triangles, vi - (resolutionHorizontal - 1), vi + 1, vi + resolutionHorizontal);
            }
        }

        private static void SetQuad(List<int> triangles, int currentIndex, int resolution)
        {
            SetTriangle(triangles, currentIndex, currentIndex + 1, currentIndex + resolution);
            SetTriangle(triangles, currentIndex + 1, currentIndex + resolution + 1, currentIndex + resolution);
        }

        private static void SetTriangle(List<int> triangles, int a, int b, int c)
        {
            triangles.Add(a);
            triangles.Add(b);
            triangles.Add(c);
        }
    }
}