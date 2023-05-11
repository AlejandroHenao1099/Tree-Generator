using UnityEngine;

namespace ProceduralMeshGeneration
{
    public partial class Plane
    {
        private MonoSpline monoSpline;
        private bool useEdgeSpline;
        private int currentEdge;


        private void UpdateVerticesSplines()
        {
            var stepX = 1f / (float)(horizontalVertices - 1);
            var stepY = 1f / (float)(verticalVertices - 1);

            for (int i = 0, n = 0; i < verticalVertices; i++)
                for (int j = 0; j < horizontalVertices; j++)
                    vertices[n++] = splineAxis2D.GetPoint(new Vector2(j * stepX, i * stepY));
        }

        public void SelectEdge(int edge)
        {
            currentEdge = edge;
            monoSpline.Clear();
            GetCorners(edge, out int cornerA, out int cornerB);
            monoSpline.UpdatePoint(vertices[cornerA], 1);
            monoSpline.UpdatePoint(vertices[cornerB], 2);
            UpdateCornersSpline();
        }

        private void UpdateEdgeSpline()
        {
            GetCorners(currentEdge, out int cornerA, out int cornerB);
            if(currentEdge == 0)
            {
                float step = 1f / (horizontalVertices - 1);
                Vector3[] targetPoints = new Vector3[horizontalVertices];
                for (int i = cornerA; i <= cornerB; i++)
                    targetPoints[i] = monoSpline.GetPoint(step * i) - vertices[i];

                for (int i = cornerA; i <= cornerB; i++)
                    vertices[i] += targetPoints[i];
            }
        }



        private void UpdateCornersSpline()
        {
            var newPoint = monoSpline[1] - monoSpline[2];
            monoSpline.UpdatePoint(newPoint, 0);
            var lastIndex = monoSpline.QuantityPoints - 1;
            newPoint = monoSpline[lastIndex - 1] - monoSpline[lastIndex - 2];
            monoSpline.UpdatePoint(newPoint, lastIndex, true);
        }








    }
}