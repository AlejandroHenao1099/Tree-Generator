using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public class DynamicSplineMesh
    {
        private Mesh mesh;
        private MeshData meshData;
        private List<Vector3> vertices, tangents;
        private List<Vector2> uv;
        private List<int> triangles;
        private DynamicSpline dynamicSpline;
        private int resolutionVertical, resolutionHorizontal;

        public Mesh GetMesh() => mesh;


        public DynamicSplineMesh(Vector3[] points, int resolutionVertical, int resolutionHorizontal)
        {
            if (points.Length < 4)
                throw new System.Exception("El tamaño de Points debe ser mayor o igual a 4");

            dynamicSpline = new DynamicSpline(points);

            vertices = new List<Vector3>((resolutionVertical + 1) * resolutionHorizontal);
            triangles = new List<int>(vertices.Count * 6);
            tangents = new List<Vector3>();
            uv = new List<Vector2>();
            mesh = new Mesh();
            this.resolutionVertical = resolutionVertical;
            this.resolutionHorizontal = resolutionHorizontal;

            for (int i = 0; i < (resolutionVertical + 1) * resolutionHorizontal; i++)
                vertices.Add(Vector3.zero);

            CreateTriangles();
        }

        public DynamicSplineMesh(DynamicSpline spline, int resolutionVertical, int resolutionHorizontal)
        {
            dynamicSpline = spline;

            vertices = new List<Vector3>();
            triangles = new List<int>();
            // tangents = new List<Vector3>();
            // uv = new List<Vector2>();
            mesh = new Mesh();
            mesh.name = "Spline";
            // meshData = new MeshData(mesh, 3);
            this.resolutionVertical = resolutionVertical;
            this.resolutionHorizontal = resolutionHorizontal;

            for (int i = 0; i < (resolutionVertical + 1) * resolutionHorizontal; i++)
                vertices.Add(Vector3.zero);

            CreateTriangles();
        }

        public void AddPoint(Vector3 point, bool updateCurve = false)
        {
            dynamicSpline.AddPoint(point, updateCurve);
        }

        public void UpdatePoint(Vector3 newPoint, int index, bool updateCurve = false)
        {
            dynamicSpline.UpdatePoint(newPoint, index, updateCurve);
        }

        public void RemovePoint(int index, bool updateCurve = false)
        {
            dynamicSpline.RemovePoint(index, updateCurve);
        }

        private void CreateRing()
        {
            var walkerSpline = new WalkerSpline(Vector3.forward, Vector3.up);
            float stepRes = 1f / (float)(resolutionVertical - 1);

            for (int i = 0; i < resolutionVertical; i++)
            {
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                var currentDirection = (dynamicSpline.GetPoint(i * stepRes)
                        - dynamicSpline.GetPoint((i + 1) * stepRes)).normalized;
                walkerSpline.UpdateDirection(currentDirection);
            }
        }

        public void UpdateSpline(DynamicSpline newSpline) => dynamicSpline = newSpline;

        public void UpdateMesh()
        {
            var walkerSpline = new WalkerSpline(Vector3.forward, Vector3.up);
            float stepRes = 1f / (float)(resolutionVertical - 1);
            float stepAngle = 360f / resolutionHorizontal;

            for (int i = 0, n = 0; i < resolutionVertical; i++)
            {
                var next = i + 1 >= resolutionVertical ? i : i + 1;
                var currentPoint = dynamicSpline.GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                var currentDirection = (dynamicSpline.GetPoint(i * stepRes)
                        - dynamicSpline.GetPoint(next * stepRes)).normalized;
                if (i + 1 < resolutionVertical)
                    walkerSpline.UpdateDirection(currentDirection);
                for (int j = 0; j < resolutionHorizontal; j++)
                {
                    float currentAngle = stepAngle * j;
                    var vertex = walkerSpline.GetCylindricalPoint(currentAngle, 1f);
                    vertices[n++] = vertex;
                }
            }
            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
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
            triangles.Add(c);
            triangles.Add(b);
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