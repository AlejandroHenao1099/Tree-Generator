using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    [ExecuteInEditMode]
    public class DynamicSpline : MonoBehaviour
    {
        public float thicknessLine;
        public float sizePoint;
        public int resolutionCurve;

        public int CountPoints
        {
            get => spline.QuantityPoints;
        }

        private MeshGenerator.DynamicSpline spline = new MeshGenerator.DynamicSpline(
            new Vector3[] { -Vector3.forward, Vector3.zero, Vector3.forward, Vector3.forward * 2 }
        );

        public Vector3 GetArrayPoint(int index)
        {
            return spline[index];
        }

        public void SetArrayPoint(int index, Vector3 newPoint)
        {
            var currPoint = spline[index];
            if (newPoint == currPoint)
                return;
            spline.UpdatePoint(newPoint, index, true);
        }

        public Vector3 GetPoint(float t)
        {
            return spline.GetPoint(t);
        }

        public void AddCurvePoint()
        {
            var newPoint = spline[spline.QuantityPoints - 1] +
                (spline[spline.QuantityPoints - 1] - spline[spline.QuantityPoints - 2]);
            spline.AddPoint(newPoint, true);
        }
    }
}