using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public partial class Plane
    {

        public Vector3 GetPositionOnRing(float t)
        {
            if (t <= 0 || t >= 1)
                return vertices[0];
            t = Mathf.Clamp01(t);
            float lerpEdge = Mathf.Lerp(0, 4, t);
            int edge = (int)lerpEdge;
            lerpEdge = lerpEdge - edge;
            return GetPositionOnEdge(lerpEdge, edge);
        }

        public Vector3 GetPositionOnEdge(float t, int edge)
        {
            t = Mathf.Clamp01(t);
            edge = Mathf.Clamp(edge, 0, 3);
            int start = 0;
            int end = 0;

            float lerp = 0;
            GetCorners(edge, out int startCorner, out int endCorner);

            if (edge == 0 || edge == 2)
            {
                if (t >= 1)
                    return vertices[edge == 0 ? endCorner : startCorner];
                else if (t <= 0)
                    return vertices[edge == 0 ? startCorner : endCorner];

                t = edge == 0 ? t : 1 - t;
                lerp = Mathf.Lerp(startCorner, endCorner, t);
                start = (int)lerp;
                end = Mathf.CeilToInt(lerp);
                lerp = lerp - start;
            }
            else
            {
                if (t >= 1)
                    return vertices[edge == 1 ? GetDesiredVertices() - 1 : 0];
                else if (t <= 0)
                    return vertices[edge == 1 ? horizontalVertices - 1 : horizontalVertices * (verticalVertices - 1)];

                t = edge == 1 ? t : 1 - t;
                lerp = Mathf.Lerp(startCorner, endCorner, t);
                start = (int)lerp;
                end = Mathf.CeilToInt(lerp);
                lerp = lerp - start;
                start = horizontalVertices * start;
                end = horizontalVertices * end;
                start = edge == 3 ? start : start + horizontalVertices - 1;
                end = edge == 3 ? end : end + horizontalVertices - 1;
            }
            return Vector3.Lerp(vertices[start], vertices[end], lerp);
        }

        public Vector3 GetPositionOnSurface(Vector2 coords)
        {
            GetSurfaceIndex(coords, out Vector2Int start, out Vector2Int end, out Vector2 lerp);
            Vector3 p00 = vertices[start.x + (horizontalVertices * start.y)];
            Vector3 p10 = vertices[end.x + (horizontalVertices * start.y)];
            Vector3 p01 = vertices[start.x + (horizontalVertices * end.y)];
            Vector3 p11 = vertices[end.x + (horizontalVertices * end.y)];
            return BilinearInterpolation(lerp, p00, p10, p01, p11);
        }

        private void GetSurfaceIndex(Vector2 t, out Vector2Int start, out Vector2Int end, out Vector2 lerp)
        {
            t.x = Mathf.Clamp01(t.x);
            t.y = Mathf.Clamp01(t.y);
            lerp = new Vector2();
            start = new Vector2Int();
            end = new Vector2Int();

            lerp.x = Mathf.Lerp(0, horizontalVertices - 1, t.x);
            lerp.y = Mathf.Lerp(0, verticalVertices - 1, t.y);
            start.x = (int)lerp.x;
            start.y = (int)lerp.y;
            end.x = Mathf.CeilToInt(lerp.x);
            end.y = Mathf.CeilToInt(lerp.y);

            lerp.x = lerp.x - Mathf.Floor(lerp.x);
            lerp.y = lerp.y - Mathf.Floor(lerp.y);

            if (t.x >= 1)
            {
                start.x = horizontalVertices - 2;
                end.x = horizontalVertices - 1;
                lerp.x = 1f;
            }
            else if (t.x <= 0)
            {
                start.x = 0;
                end.x = 1;
                lerp.x = 0f;
            }
            if (t.y >= 1)
            {
                start.y = verticalVertices - 2;
                end.y = verticalVertices - 1;
                lerp.y = 1f;
            }
            else if (t.y <= 0)
            {
                start.y = 0;
                end.y = 1;
                lerp.y = 0f;
            }
        }

        private Vector3 BilinearInterpolation(Vector2 t, Vector3 p00, Vector3 p10, Vector3 p01, Vector3 p11)
        {
            float x = Mathf.Clamp01(t.x);
            float y = Mathf.Clamp01(t.y);
            Vector3 c0 = Vector3.Lerp(p00, p10, x);
            Vector3 c1 = Vector3.Lerp(p01, p11, x);
            return Vector3.Lerp(c0, c1, y);
        }

        private void GetCorners(int edge, out int cornerA, out int cornerB)
        {
            switch (edge)
            {
                case 0:
                    cornerA = 0;
                    cornerB = horizontalVertices - 1;
                    return;
                case 2:
                    cornerA = horizontalVertices * (verticalVertices - 1);
                    cornerB = GetDesiredVertices() - 1;
                    return;
                case 1:
                case 3:
                    cornerA = 0;
                    cornerB = verticalVertices - 1;
                    return;
                default:
                    cornerA = 0;
                    cornerB = horizontalVertices - 1;
                    return;
            }
        }
    }
}