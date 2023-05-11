using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public struct TriFace
    {
        public int a, b, c;
        public Edge AB, AC, BC;

        public Vector3 GetNormal(List<Vector3> vertices)
        {
            UnityEngine.Plane plane = new UnityEngine.Plane(vertices[a], vertices[b], vertices[c]);
            return plane.normal;
        }

        public Vector3 GetNearestPoint(List<Vector3> vertices, Vector3 point)
        {
            UnityEngine.Plane plane = new UnityEngine.Plane(vertices[a], vertices[b], vertices[c]);
            return plane.ClosestPointOnPlane(point);
        }

        public Vector3 GetCenter(List<Vector3> vertices)
        {
            Vector3 Va = vertices[a];
            Vector3 Vb = vertices[b];
            Vector3 Vc = vertices[c];

            Vector3 midPoint = Vector3.Lerp(Va, Vb, 0.5f);
            return Vector3.Lerp(midPoint, Vc, 0.5f);
        }
    }
}