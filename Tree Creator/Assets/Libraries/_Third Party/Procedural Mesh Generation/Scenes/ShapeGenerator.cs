using UnityEngine;
using MeshGenerator;

namespace ProceduralMeshGeneration
{
    public class ShapeGenerator : MonoBehaviour
    {
        Plane plane;
        public int horizontalFaces, verticalFaces;
        public float height, width;

        public SpaceType axisX, axisY, axisZ;
        public SplineAxis2D splineAxis2D;
        public MonoMesh monoMesh;
        public MonoSpline monoSpline;
        public bool useSpline;
        public bool useEdgeSpline;

        public TypeMovement typeMovement;
        [Range(0f, 1f)]
        public float xCoord;
        [Range(0f, 1f)]
        public float yCoord;
        public Transform objectToMove;
        [Range(0, 3)]
        public int edge;
        [Range(0, 1)]
        public float t;

        private bool xReady, yReady;

        private bool onStart;


        private void Start()
        {
            plane = new Plane(splineAxis2D, monoMesh, monoSpline);
            splineAxis2D.shape = plane;
            monoSpline.shape = plane;
            GetComponent<MeshFilter>().mesh = plane.Mesh;
            monoMesh.UpdateMesh(plane.Mesh);
            onStart = true;
        }

        private void OnValidate()
        {
            if (onStart == false)
                return;
            plane.HorizontalFaces = horizontalFaces;
            plane.VerticalFaces = verticalFaces;
            plane.Height = height;
            plane.Width = width;

            plane.AxisX = axisX;
            plane.AxisY = axisY;
            plane.AxisZ = axisZ;
            plane.UseSpline = useSpline;
            plane.UseEdgeSpline = useEdgeSpline;
            MoveObject();

        }

        private void MoveObject()
        {
            switch (typeMovement)
            {
                case TypeMovement.Surface:
                    objectToMove.position = plane.GetPositionOnSurface(new Vector2(xCoord, yCoord));
                    break;
                case TypeMovement.Edge:
                    objectToMove.position = plane.GetPositionOnEdge(t, edge);
                    break;
                case TypeMovement.Ring:
                    objectToMove.position = plane.GetPositionOnRing(t);
                    break;
            }
        }

        public void UpdatePlane()
        {
            if (useSpline == false)
                return;
            if (xReady == false)
                xReady = true;
            else if (yReady == false)
                yReady = true;
            if (xReady == true && yReady == true)
            {
                plane.Update();
                xReady = false;
                yReady = false;
            }
        }
    }
}

public enum TypeMovement
{
    Surface, Edge, Ring
}