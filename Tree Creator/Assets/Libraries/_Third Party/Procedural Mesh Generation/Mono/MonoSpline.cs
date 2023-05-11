using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public class MonoSpline : MonoBehaviour
    {
        private MeshGenerator.DynamicSpline spline;

        public Material materialCube;
        public float sizePoint;
        public float widthLine;
        public int resolutionCurve;
        public bool addPointToSpline;

        private List<Transform> points;

        public LineRenderer lineRenderer;
        public IShape shape;

        public int QuantityPoints { get => spline.QuantityPoints; }

        public Vector3 this[int index]
        {
            get => points[index].localPosition;
        }

        private void Awake()
        {
            points = new List<Transform>();
            lineRenderer.startWidth = lineRenderer.endWidth = widthLine;
            spline = new MeshGenerator.DynamicSpline(new Vector3[] { Vector3.one, Vector3.one * 2,
                Vector3.one * 3, Vector3.one * 4});
        }

        private void Update()
        {
            CheckPoints();
        }

        private void CheckPoints()
        {
            var thereChange = false;
            for (int i = 0; i < points.Count; i++)
            {
                var point = points[i].localPosition;
                var splinePoint = spline[i];
                if (point != splinePoint)
                {
                    UpdatePoint(point, i, true);
                    thereChange = true;
                }
            }
            if (thereChange)
                UpdateLineRenderer();
        }

        public Vector3 GetPoint(float t)
        {
            return spline.GetPoint(t);
        }

        public void UpdatePoint(Vector3 newValue, int index, bool updateSpline = false)
        {
            spline.UpdatePoint(newValue, index, updateSpline);
            UpdateTransPoint(index, newValue);
            if (updateSpline == true)
                shape.Update();
        }

        private void UpdateTransPoint(int index, Vector3 newPoint)
        {
            points[index].localPosition = newPoint;
        }

        private void IncreaseResolutionSpline()
        {
            spline.IncreaseResolution(1);
            points.Add(CreatePoint());
            for (int i = 0; i < points.Count; i++)
                points[i].localPosition = spline[i];
        }

        private void OnValidate()
        {
            if (addPointToSpline == true)
            {
                IncreaseResolutionSpline();
                addPointToSpline = false;
            }
        }

        private Transform CreatePoint()
        {
            var newPoint = new GameObject("Point");
            newPoint.AddComponent<MeshFilter>().mesh = MeshGenerator.Cube.Create(2, 2, 2, sizePoint, sizePoint, sizePoint);
            newPoint.AddComponent<MeshRenderer>().material = materialCube;
            newPoint.AddComponent<BoxCollider>().isTrigger = true;
            newPoint.AddComponent<Rigidbody>().isKinematic = true;
            newPoint.transform.SetParent(transform);
            newPoint.transform.localScale = Vector3.one;
            return newPoint.transform;
        }

        private void UpdateLineRenderer()
        {
            lineRenderer.positionCount = resolutionCurve;
            var posLine = new Vector3[resolutionCurve];
            float step = 1f / (resolutionCurve - 1f);

            for (int i = 0; i < resolutionCurve; i++)
                posLine[i] = spline.GetPoint(i * step) + Vector3.up * 0.05f;
            lineRenderer.SetPositions(posLine);
        }

        public void Clear()
        {
            spline.Clear();
            for (int i = 0; i < 4; i++)
                spline.UpdatePoint(Vector3.one * i, i);

            for (int i = 0; i < points.Count; i++)
                Destroy(points[i].gameObject);
            points.Clear();
            for (int i = 0; i < spline.QuantityPoints; i++)
                points.Add(CreatePoint());

            for (int i = 0; i < spline.QuantityPoints; i++)
                points[i].localPosition = spline[i];
            UpdateLineRenderer();
        }

        // private ref DynamicSpline GetSplineAxis(Axis axis)
        // {
        //     if (axis == Axis.X)
        //         return ref spline;
        //     else
        //         return ref axisY;
        // }

    }
}