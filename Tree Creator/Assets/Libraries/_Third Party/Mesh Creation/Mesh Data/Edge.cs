using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public struct Edge
    {
        public Vertex from;
        public Vertex to;

        public Edge(Vertex from, Vertex to)
        {
            this.from = from;
            this.to = to;
        }
    }
}