using UnityEngine;

namespace ProceduralMeshGeneration
{
    public partial class Plane
    {
        public Mesh Mesh { get => mesh; }

        public int VerticalFaces
        {
            get => verticalFaces;
            set
            {
                if (value == verticalFaces)
                    return;
                int from = verticalFaces;
                verticalFaces = Mathf.Max(value, 1);
                verticalVertices = verticalFaces + 1;
                UpdateResolutionMesh();
                UpdateVerticalFaces(from, verticalFaces);
                Update();
            }
        }

        public int HorizontalFaces
        {
            get => horizontalFaces;
            set
            {
                if (value == horizontalFaces)
                    return;
                horizontalFaces = Mathf.Max(value, 1);
                horizontalVertices = horizontalFaces + 1;
                UpdateResolutionMesh();
                UpdateHorizontalFaces();
                Update();
            }
        }

        public float Width
        {
            get => width;
            set
            {
                width = Mathf.Max(value, 0.1f);
                Update();
            }
        }

        public float Height
        {
            get => height;
            set
            {
                height = Mathf.Max(value, 0.1f);
                Update();
            }
        }

        public int FaceRingSelected
        {
            get => faceRingSelected;
            set
            {
                if (faceRingSelected == value)
                    return;
                faceRingSelected = value;
            }
        }

        public int VertexRingSelected
        {
            get => vertexRingSelected;
            set
            {
                if (vertexRingSelected == value)
                    return;
                vertexRingSelected = value;
            }
        }

        public SpaceType AxisX
        {
            get => axisX;
            set
            {
                if (axisX == value)
                    return;
                axisX = value;
                Update();
            }
        }

        public SpaceType AxisY
        {
            get => axisY;
            set
            {
                if (axisY == value)
                    return;
                axisY = value;
                Update();
            }
        }

        public SpaceType AxisZ
        {
            get => axisZ;
            set
            {
                if (axisZ == value)
                    return;
                axisZ = value;
                Update();
            }
        }

        public bool UseSpline
        {
            set
            {
                if (value == useSpline)
                    return;
                useSpline = value;
                Update();
            }
        }

        public bool UseEdgeSpline
        {
            set
            {
                if (value == useEdgeSpline)
                    return;
                useEdgeSpline = value;
                if (value == true)
                {
                    SelectEdge(0);
                }
                Update();
            }
        }
    }
}