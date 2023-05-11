using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public struct Edge
    {
        public int start, end;

        public Vector3 GetCenter(List<Vector3> vertices)
        {
            Vector3 Va = vertices[start];
            Vector3 Vb = vertices[end];
            return Vector3.Lerp(Va, Vb, 0.5f);
        }
    }
}