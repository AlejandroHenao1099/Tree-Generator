using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public class MeshData
    {
        private Mesh mesh;
        private Vertex[] vertices;
        private Face[] faces;

        private Dictionary<Vector3Int, List<int>> vertexPerSection;

        public MeshData(Mesh mesh, int resolutionCaching = 1)
        {
            this.mesh = mesh;
            CreateVertex();
            CreateFaces();
            CreateSectionalBounds(resolutionCaching);
        }

        private void CreateVertex()
        {
            var len = mesh.vertices.Length;
            vertices = new Vertex[len];
            for (int i = 0; i < len; i++)
                vertices[i] = new Vertex(mesh.vertices[i], i);
        }

        private void CreateFaces()
        {
            var len = mesh.triangles.Length;
            faces = new Face[len / 3];
            for (int ti = 0, n = 0; ti < len; ti += 3, n++)
                faces[n] = new Face(mesh.triangles[ti], mesh.triangles[ti + 1], mesh.triangles[ti + 2], n,
                CalculateCenter(vertices[ti].position, vertices[ti + 1].position, vertices[ti + 2].position));
        }

        private void CreateSectionalBounds(int resolutionCaching)
        {
            if (resolutionCaching <= 1)
                return;
            vertexPerSection = new Dictionary<Vector3Int, List<int>>();
            Bounds bounds = mesh.bounds;

            Vector3 size = bounds.size / (float)resolutionCaching;
            Vector3 midleSize = size / 2f;

            for (int y = 0; y < resolutionCaching; y++)
            {
                for (int z = 0; z < resolutionCaching; z++)
                {
                    for (int x = 0; x < resolutionCaching; x++)
                    {
                        Vector3 floatSection = new Vector3(x * size.x, y * size.y, z * size.z) + midleSize;
                        Vector3Int section = new Vector3Int(Mathf.RoundToInt(floatSection.x),
                             Mathf.RoundToInt(floatSection.y), Mathf.RoundToInt(floatSection.z));
                        vertexPerSection.Add(section, new List<int>());
                    }
                }
            }
            SearchNearestSectionToFace();
        }

        private void SearchNearestSectionToFace()
        {
            for (int i = 0; i < faces.Length; i++)
            {
                Vector3Int nearestSection = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
                float nearestDistance = float.MaxValue;
                Vector3 center = faces[i].center;
                foreach (var section in vertexPerSection)
                {
                    if (Vector3.SqrMagnitude(section.Key - center) < nearestDistance)
                    {
                        nearestSection = section.Key;
                        nearestDistance = Vector3.SqrMagnitude(section.Key - nearestSection);
                    }
                }
                vertexPerSection[nearestSection].Add(i);
            }
        }

        public Vector3 GetNearestPoint(Vector3 point)
        {
            var face = GetNearestFace(point);
            return CalculatePointOnFace(face, point);
        }

        public Vector3 GetNearestNormal(Vector3 point)
        {
            var face = GetNearestFace(point);
            return CalculateNormalOnFace(face);
        }

        public float GetNormalizedPosition(Vector3 point)
        {
            var face = GetNearestFace(point);
            return CalculateNormalizedPosition(face, point);
        }

        public void GetNearestPointAndNormal(Vector3 point, out Vector3 nearestPoint, out Vector3 nearestNormal)
        {
            var nearFace = GetNearestFace(point);
            nearestPoint = CalculatePointOnFace(nearFace, point);
            nearestNormal = CalculateNormalOnFace(nearFace);
        }

        public void GetNearestPointNormalNormalizedPosition(Vector3 point, out Vector3 nearestPoint, 
            out Vector3 nearestNormal, out float normalizedPosition)
        {
            var nearFace = GetNearestFace(point);
            nearestPoint = CalculatePointOnFace(nearFace, point);
            nearestNormal = CalculateNormalOnFace(nearFace);
            normalizedPosition = CalculateNormalizedPosition(nearFace, point);
        }

        private Face GetNearestFace(Vector3 point)
        {
            Vector3Int nearestSection = new Vector3Int(int.MaxValue, int.MaxValue, int.MaxValue);
            float nearestDistance = float.MaxValue;
            foreach (var section in vertexPerSection)
            {
                if (Vector3.SqrMagnitude(section.Key - point) < nearestDistance)
                {
                    nearestSection = section.Key;
                    nearestDistance = Vector3.SqrMagnitude(section.Key - nearestSection);
                }
            }

            int nearestFace = -1;
            nearestDistance = float.MaxValue;
            var len = vertexPerSection[nearestSection].Count;
            for (int i = 0; i < len; i++)
            {
                var currentPoint = faces[vertexPerSection[nearestSection][i]].center;
                if (Vector3.SqrMagnitude(currentPoint - point) < nearestDistance)
                {
                    nearestFace = i;
                    nearestDistance = Vector3.SqrMagnitude(currentPoint - point);
                }
            }
            return faces[vertexPerSection[nearestSection][nearestFace]];
            // if (normal)
            //     return CalculateNormalOnFace(faces[vertexPerSection[nearestSection][nearestFace]]);
            // return CalculatePointOnFace(faces[vertexPerSection[nearestSection][nearestFace]], point);
        }

        // private Vector3 GetCenterFace(Face face)
        // {
        //     Vector3 a = vertices[face.a].position;
        //     Vector3 b = vertices[face.b].position;
        //     Vector3 c = vertices[face.c].position;
        //     Vector3 center = Vector3.Lerp(Vector3.Lerp(a, b, 0.5f), Vector3.Lerp(a, c, 0.5f), 0.5f);
        //     center = Vector3.Lerp(Vector3.Lerp(c, b, 0.5f), center, 0.5f);
        //     return center;
        // }

        private Vector3 CalculateCenter(Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 center = Vector3.Lerp(Vector3.Lerp(a, b, 0.5f), Vector3.Lerp(a, c, 0.5f), 0.5f);
            center = Vector3.Lerp(Vector3.Lerp(c, b, 0.5f), center, 0.5f);
            return center;
        }

        private Vector3 CalculatePointOnFace(Face face, Vector3 point)
        {
            UnityEngine.Plane plane = new UnityEngine.Plane(vertices[face.a].position, vertices[face.b].position, vertices[face.c].position);
            return plane.ClosestPointOnPlane(point);
        }

        private Vector3 CalculateNormalOnFace(Face face)
        {
            UnityEngine.Plane plane = new UnityEngine.Plane(vertices[face.a].position, vertices[face.b].position, vertices[face.c].position);
            return plane.normal;
        }

        private float CalculateNormalizedPosition(Face face, Vector3 point)
        {
            UnityEngine.Plane plane = new UnityEngine.Plane(vertices[face.a].position, vertices[face.b].position, vertices[face.c].position);
            return plane.GetDistanceToPoint(point);
        }
    }
}