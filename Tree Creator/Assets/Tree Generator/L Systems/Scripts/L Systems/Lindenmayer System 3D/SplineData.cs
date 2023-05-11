using System.Collections.Generic;
using UnityEngine;

namespace LindenmayerSystems
{
    public struct SplineData
    {
        public List<Vector3> points;
        public List<float> widths;

        public SplineData(int size)
        {
            points = new List<Vector3>();
            widths = new List<float>();
        }

        public void AddPoint(Vector3 point) => points.Add(point);

        public void AddWidth(float width) => widths.Add(width);

        public float GetLastWidth() => widths[widths.Count - 1];

        public Vector3 GetLastPoint() => points[points.Count - 1];
    }
}