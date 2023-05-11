using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public class MonoMesh : MonoBehaviour
    {
        public float sizeVertex = 0.1f;
        public int layerVertex;
        private float prevSize = 0.1f;
        public Material materialVertex;
        public Material materialVertexSelected;
        private List<Transform> vertices;
        // private List<Transform> edges;
        // private List<Transform> faces;

        private Mesh targetMesh;

        private void OnValidate()
        {
            if(sizeVertex != prevSize)
            {
                UpdateSizeVertices();
                prevSize = sizeVertex;
            }
        }

        private void Awake()
        {
            vertices = new List<Transform>();
        }

        // private void Update()
        // {
        //     CheckMonoVertices();
        // }

        public void UpdateVertices()
        {
            UpdateQuantityVertices();
            UpdateMonoVertices();
        }

        private void CheckMonoVertices()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                var monoVer = vertices[i].localPosition;
                var meshVer = targetMesh.vertices[i];
                if (monoVer != meshVer)
                    targetMesh.vertices[i] = monoVer;
            }
        }

        private void UpdateMonoVertices()
        {
            var len = targetMesh.vertices.Length;
            for (int i = 0; i < len; i++)
                vertices[i].localPosition = targetMesh.vertices[i];
        }

        private void UpdateMeshVertices()
        {
            var len = targetMesh.vertices.Length;
            for (int i = 0; i < len; i++)
                targetMesh.vertices[i] = vertices[i].localPosition;
        }

        private void UpdateSizeVertices()
        {
            for (int i = 0; i < vertices.Count; i++)
                vertices[i].localScale = Vector3.one * sizeVertex;
        }

        public void UpdateMesh(Mesh mesh)
        {
            targetMesh = mesh;
            vertices.Clear();
            UpdateQuantityVertices();
            UpdateMonoVertices();
            UpdateSizeVertices();
        }

        private void UpdateQuantityVertices()
        {
            var desiredVertices = targetMesh.vertices.Length;
            var currentVertices = vertices.Count;

            if (desiredVertices > currentVertices)
            {
                for (int i = currentVertices; i < desiredVertices; i++)
                {
                    var vertex = CreateVertex();
                    vertex.localScale = Vector3.one * sizeVertex;
                    vertices.Add(vertex);
                }
            }
            else if (desiredVertices < currentVertices)
            {
                for (int i = currentVertices; i != desiredVertices; i--)
                {
                   var currVertex = vertices[vertices.Count - 1];
                   vertices.RemoveAt(vertices.Count - 1);
                   Destroy(currVertex.gameObject);
                }
                // var quantityToRemove = currentVertices - desiredVertices;
                // var index = currentVertices - quantityToRemove;
                // vertices.RemoveRange(index, quantityToRemove);
            }
        }

        public void SelectVertices(int[] verticesToSelect)
        {
            for (int i = 0; i < verticesToSelect.Length; i++)
            {
                var currVer = verticesToSelect[i];
                vertices[currVer].GetComponent<MeshRenderer>().material = materialVertexSelected;
            }
        }

        private Transform CreateVertex()
        {
            var newPoint = new GameObject("Point");
            newPoint.AddComponent<MeshFilter>().mesh = MeshGenerator.Cube.Create(2, 2, 2, 1, 1, 1);
            newPoint.AddComponent<MeshRenderer>().material = materialVertex;
            newPoint.AddComponent<BoxCollider>().isTrigger = true;
            newPoint.AddComponent<Rigidbody>().isKinematic = true;
            newPoint.transform.SetParent(transform);
            newPoint.transform.localScale = Vector3.one;
            newPoint.layer = layerVertex;
            return newPoint.transform;
        }
    }
}