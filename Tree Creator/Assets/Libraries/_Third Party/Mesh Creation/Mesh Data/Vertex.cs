using UnityEngine;

namespace MeshGenerator
{
    public struct Vertex
    {
        public Vector3 position;
        public int idVertex;

        public Vertex(Vector3 position, int id)
        {
            this.position = position;
            this.idVertex = id;
        }
    }
}