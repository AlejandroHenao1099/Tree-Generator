using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    namespace LeavesCreator
    {
        [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
        public class LeafShapeCreator : MonoBehaviour
        {
            public float sizeCube;
            public Color colorVertices;

            public int resolutionLeaf = 3;

            [Range(1, 180)]
            public int angle = 90;
            [Min(0)]
            public float height = 1;
            [Min(0)]
            public float widht = 1;
            [Range(0f, 1f)]
            public float normalizePosition = 0f;



            private List<Vector3> vertices;
            private List<int> triangles;
            private Mesh mesh;

            private void Awake()
            {
                mesh = GetComponent<Mesh>();
                vertices = new List<Vector3>();
                // triangles = new List<int>();
                UpdateQuantityVertices();
                UpdateVertices();
            }

            private void UpdateQuantityVertices()
            {
                int newVerts = GetDesiredVertices();
                int currentVerts = vertices.Count;
                if (currentVerts < newVerts)
                    for (int i = currentVerts; i < newVerts; i++)
                        vertices.Add(Vector3.zero);
                else if (currentVerts > newVerts)
                    for (int i = currentVerts; i != newVerts; i--)
                        vertices.RemoveAt(vertices.Count - 1);
            }

            private void UpdateVertices()
            {
                float stepAngle = ((float)angle / resolutionLeaf) * Mathf.Deg2Rad;
                float midleWidth = widht / 2f;
                float midleHeight = height / 2f;
                // Vector3 offset = Vector3.up * midleHeight * (1f - normalizePosition);
                var n = 0;
                vertices[n++] = Vector3.zero;
                // vertices[n++] = new Vector3(0, midleHeight, 0) + offset;
                vertices[n++] = new Vector3(0, midleHeight, 0) * (1 + Mathf.Cos(0));

                for (int i = 1; i <= resolutionLeaf; i++)
                    vertices[n++] =
                        new Vector3(Mathf.Sin(stepAngle * i) * midleWidth,
                            Mathf.Cos(stepAngle * i) * midleHeight, 0) * (1 + Mathf.Cos(stepAngle * i));
                // Mathf.Cos(stepAngle * i) * midleHeight, 0) + offset;

                for (int i = 1; i <= resolutionLeaf; i++)
                    vertices[n++] =
                        new Vector3(Mathf.Sin(-stepAngle * i) * midleWidth,
                            Mathf.Cos(-stepAngle * i) * midleHeight, 0) * (1 + Mathf.Cos(stepAngle * i));
                // Mathf.Cos(-stepAngle * i) * midleHeight, 0) + offset;

                // if (angle >= 180)
                //     vertices[n++] = new Vector3(0, -bottomHeight, 0);

            }

            private int GetDesiredVertices()
            {
                if (angle >= 180)
                    return (resolutionLeaf * 2) + 3;
                return (resolutionLeaf * 2) + 2;
            }

            private void OnValidate()
            {
                if (vertices == null || vertices.Count == 0) return;
                UpdateQuantityVertices();
                UpdateVertices();
            }

            private void OnDrawGizmos()
            {
                if (vertices == null || vertices.Count == 0) return;
                Gizmos.color = colorVertices;
                for (int i = 0; i < vertices.Count; i++)
                    Gizmos.DrawCube(vertices[i], Vector3.one * sizeCube);
            }



        }
    }
}