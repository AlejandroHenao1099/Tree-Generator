using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public struct Vertex
    {
        public Vector3 point;
        public int index;
        public List<int> faces;
        public List<int> edges;
        
    }
}