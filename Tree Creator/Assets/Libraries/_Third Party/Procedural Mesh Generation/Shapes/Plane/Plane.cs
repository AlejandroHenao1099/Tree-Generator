using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace ProceduralMeshGeneration
{
    public partial class Plane : MeshData, IShape
    {
        private int verticalFaces = 1;
        private int horizontalFaces = 1;
        private int verticalVertices = 2;
        private int horizontalVertices = 2;
        private float height = 1f;
        private float width = 1f;

        private int vertexRingSelected = -1;
        private int faceRingSelected = -1;


        private Mesh mesh;
        // private List<QuadFace> faces;
        // private List<Edge> edges;
        // private List<Vertex> vertex;
        private SpaceType axisX, axisY, axisZ;

        private SplineAxis2D splineAxis2D;
        private MonoMesh monoMesh;
        private bool useSpline;

        public Plane(SplineAxis2D splineAxis2D, MonoMesh monoMesh, MonoSpline monoSpline) : base()
        {
            mesh = new Mesh();
            mesh.name = "Plane";
            this.splineAxis2D = splineAxis2D;
            this.monoMesh = monoMesh;
            this.monoSpline = monoSpline;
            // vertex = new List<Vertex>();
            // edges = new List<Edge>();
            // faces = new List<QuadFace>();
            // InitializeMeshData();
            UpdateResolutionMesh();
            UpdateVertices();
            UpdateHorizontalFaces();
            UpdateUnityMeshData();
        }

        public void Update()
        {
            UpdateVertices();
            UpdateUnityMeshData();
            monoMesh.UpdateVertices();
        }

        private void UpdateVertices()
        {
            // if (useSpline == true)
            //     UpdateVerticesSplines();
            if (useEdgeSpline == true)
                UpdateEdgeSpline();
            else
                UpdateVerticesDefault();
        }

        private void UpdateVerticesDefault()
        {
            var middle = new Vector3(Width / 2f, 0, Height / 2f);

            var normalizeWidthStep = 2f / (float)(horizontalVertices - 1);
            var normalizeHeightStep = 2f / (float)(verticalVertices - 1);

            for (int i = 0, n = 0; i < verticalVertices; i++)
                for (int j = 0; j < horizontalVertices; j++)
                {
                    var currPoint = new Vector3(-1f + (normalizeWidthStep * j), 0, -1f + (normalizeHeightStep * i));
                    var currHor = new Vector3(-1f + (normalizeWidthStep * j), 0, 0);
                    var currVer = new Vector3(0, 0, -1f + (normalizeHeightStep * i));
                    var transformedPoint = GetTransformedVertex(currPoint);
                    Vector3 finalVertex = GetFinalvertex(currHor, currVer, transformedPoint);
                    finalVertex.x *= middle.x;
                    finalVertex.z *= middle.z;
                    vertices[n++] = finalVertex;
                }
        }



        // private void UpdateSizeVertex()
        // {
        //     var desiredSize = GetDesiredVertices();

        //     int currentSize = vertex.Count;
        //     if (desiredSize > currentSize)
        //         for (int i = currentSize; i < desiredSize; i++)
        //             vertex.Add(new Vertex());
        //     else if (desiredSize < currentSize)
        //         for (int i = currentSize; i != desiredSize; i--)
        //             meshList.RemoveAt(meshList.Count - 1);

        // }

        private void SelectVertexRing()
        {

        }

        // private void InitializeMeshData()
        // {
        //     var desiredVertices = GetDesiredVertices();
        //     for (int i = 0; i < desiredVertices; i++)
        //     {
        //         var newVertex = new Vertex();
        //         newVertex.index = i;
        //         newVertex.faces = new List<int>();
        //         newVertex.edges = new List<int>();
        //         vertex.Add(new Vertex());
        //     }

        //     int vi = 0;
        //     int fi = 0;
        //     for (int i = 0; i < VerticalFaces; i++, vi++)
        //     {
        //         for (int j = 0; j < HorizontalFaces; j++, vi++)
        //         {
        //             var face = new QuadFace();

        //             face.a = vi;
        //             face.b = vi + 1;
        //             face.c = vi + horizontalVertices;
        //             face.d = vi + horizontalVertices + 1;

        //             vertex[face.a].faces.Add(fi);
        //             vertex[face.b].faces.Add(fi);
        //             vertex[face.c].faces.Add(fi);
        //             vertex[face.d].faces.Add(fi);
        //             faces.Add(face);
        //             fi++;
        //         }
        //     }

        //     int ei = 0;
        //     vi = 0;

        //     for (int i = 0; i < VerticalFaces + 1; i++, vi++)
        //     {
        //         for (int j = 0; j < HorizontalFaces; j++, vi++)
        //         {
        //             var edge = new Edge();
        //             edge.start = vi;
        //             edge.end = vi + 1;
        //             vertex[edge.start].edges.Add(ei);
        //             vertex[edge.end].edges.Add(ei);
        //             edges.Add(edge);
        //             ei++;
        //         }
        //     }

        //     vi = 0;
        //     for (int i = 0; i < VerticalFaces; i++)
        //     {
        //         for (int j = 0; j < HorizontalFaces + 1; j++, vi++)
        //         {
        //             var edge = new Edge();
        //             edge.start = vi;
        //             edge.end = vi + horizontalVertices;
        //             vertex[edge.start].edges.Add(ei);
        //             vertex[edge.end].edges.Add(ei);
        //             edges.Add(edge);
        //             ei++;
        //         }
        //     }
        // }


    }
}