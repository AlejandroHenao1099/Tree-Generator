using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public struct QuadFace
    {
        public int a, b, c, d;
        public Edge AB, AC, BD, CD;

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
            Vector3 Vd = vertices[d];

            Vector3 midPointAB = Vector3.Lerp(Va, Vb, 0.5f);
            Vector3 midPointCD = Vector3.Lerp(Vc, Vd, 0.5f);
            return Vector3.Lerp(midPointAB, midPointCD, 0.5f);
        }
    }
}