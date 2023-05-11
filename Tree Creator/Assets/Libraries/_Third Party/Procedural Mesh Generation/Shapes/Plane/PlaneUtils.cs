using UnityEngine;

namespace ProceduralMeshGeneration
{
    public partial class Plane
    {
        private int GetDesiredVertices() => horizontalVertices * verticalVertices;
        private int GetDesiredTriangles() => HorizontalFaces * VerticalFaces * 6;

        private int GetVertexIndex(int horizontalRow) => horizontalRow * horizontalVertices;
        private int GetTriangleIndex(int horizontalRow) => horizontalRow * horizontalFaces * 6;

        private Vector3 GetTransformedVertex(Vector3 point) => SpaceGenerator.GetPoint(point, AxisX, AxisY, AxisZ);

        private Vector3 GetFinalvertex(Vector3 horizontalPoint, Vector3 verticalPoint, Vector3 transformedPoint)
        {
            Vector3 vertex = horizontalPoint + verticalPoint;

            if (axisX != SpaceType.Lineal)
                vertex += new Vector3(0, 0, transformedPoint.x);
            if (axisZ != SpaceType.Lineal)
                vertex += new Vector3(transformedPoint.z, 0, 0);
            if (axisY != SpaceType.Lineal)
            {
                float xVal = Mathf.Clamp(horizontalPoint.x, -1f, 1f);
                xVal = SpaceGenerator.GetTransformedValue(xVal, axisY);
                float zVal = Mathf.Clamp(verticalPoint.z, -1f, 1f);
                zVal = SpaceGenerator.GetTransformedValue(zVal, axisY);
                vertex += new Vector3(0, xVal + zVal, 0);
            }
            return vertex;
        }

        private void UpdateRings(int from, int to)
        {
            int vi = (horizontalVertices * from) + from;
            int maxVi = (horizontalVertices * to) + to;


        }

        private int GetMaxVertexRings()
        {
            int min = Mathf.Min(HorizontalFaces, VerticalFaces);
            if (min <= 2)
                return min;
            return min - (min % 2) - 1;
        }
    }
}

public enum SpaceType
{
    Lineal, Quadratic, Cubic, Sin, Cos, SquareRoot, SemiCircle
}