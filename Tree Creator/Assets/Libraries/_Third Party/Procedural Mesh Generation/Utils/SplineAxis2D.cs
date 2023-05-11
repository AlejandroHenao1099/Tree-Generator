using System.Collections.Generic;
using UnityEngine;

namespace MeshGenerator
{
    public class SplineAxis2D : MonoBehaviour
    {
        private DynamicSpline axisX;
        private DynamicSpline axisY;

        public Material materialCube;
        public float sizePoint;
        public float widthLine;
        public int resolutionCurve;
        public bool addPointToAxisX, addPointToAxisY;

        public TypeAxis typeAxisX, typeAxisY;
        private TypeAxis prevTypeAxisX, prevTypeAxisY;

        private List<Transform> pointsAxisX;
        private List<Transform> pointsAxisY;

        public LineRenderer lineRendererX, lineRendererY;

        public IShape shape;

        private void Awake()
        {
            pointsAxisX = new List<Transform>();
            pointsAxisY = new List<Transform>();
            CreateAxis(Vector3.forward, pointsAxisY, out axisY);
            CreateAxis(Vector3.right, pointsAxisX, out axisX);
            lineRendererX.startWidth = lineRendererX.endWidth = widthLine;
            lineRendererY.startWidth = lineRendererY.endWidth = widthLine;

            UpdateLine(Axis.X);
            UpdateLine(Axis.Y);
        }

        private void Update()
        {
            CheckPoints();
        }

        private void CheckPoints()
        {
            int origin = pointsAxisX.Count / 2;
            var thereChange = false;
            for (int i = 0; i < pointsAxisX.Count; i++)
            {
                if (i == origin)
                    continue;
                var point = pointsAxisX[i].localPosition;
                var splinePoint = axisX[i];
                if (point != splinePoint)
                {
                    UpdatePoint(i, point, Axis.X, typeAxisX);
                    thereChange = true;
                }
            }
            pointsAxisX[origin].localPosition = Vector3.zero;
            if (thereChange)
                UpdateLine(Axis.X);

            origin = pointsAxisY.Count / 2;

            thereChange = false;
            for (int i = 0; i < pointsAxisY.Count; i++)
            {
                if (i == origin)
                    continue;
                var point = pointsAxisY[i].localPosition;
                var splinePoint = axisY[i];
                if (point != splinePoint)
                {
                    UpdatePoint(i, point, Axis.Y, typeAxisY);
                    thereChange = true;
                }
            }
            pointsAxisY[origin].localPosition = Vector3.zero;
            if (thereChange)
                UpdateLine(Axis.Y);
        }

        public Vector3 GetPoint(Vector2 t)
        {
            var xValue = axisX.GetPoint(t.x);
            var yValue = axisY.GetPoint(t.y);
            return xValue + yValue;
        }

        public void UpdatePoint(int index, Vector3 newValue, Axis axis, TypeAxis typeAxis)
        {
            if (typeAxis == TypeAxis.Symetric || typeAxis == TypeAxis.Asymetric)
            {
                Vector3 mirrorValue = Vector3.zero;
                if (axis == Axis.X && typeAxis == TypeAxis.Symetric)
                    mirrorValue = new Vector3(-newValue.x, newValue.y, newValue.z);
                else if (axis == Axis.Y && typeAxis == TypeAxis.Symetric)
                    mirrorValue = new Vector3(newValue.x, newValue.y, -newValue.z);
                else
                    mirrorValue = -newValue;

                UpdateSymetric(index, newValue, mirrorValue, axis);
                shape.Update();
            }
            else
            {
                UpdateFreeAxis(index, newValue, axis);
                shape.Update();
            }
        }

        private void UpdateSymetric(int index, Vector3 newValue, Vector3 mirrorValue, Axis axis)
        {
            ref DynamicSpline splineAxis = ref GetSplineAxis(axis);
            int mirrorIndex = 0;
            int origin = splineAxis.QuantityPoints / 2;
            if (origin == index)
                return;

            if (index > origin)
                mirrorIndex = origin - (index - origin);
            else if (index < origin)
                mirrorIndex = origin + (origin - index);


            splineAxis.UpdatePoint(newValue, index);
            splineAxis.UpdatePoint(mirrorValue, mirrorIndex);

            UpdateTransPoint(index, newValue, axis);
            UpdateTransPoint(mirrorIndex, mirrorValue, axis);

            splineAxis.UpdateSpline();
        }

        private void UpdateFreeAxis(int index, Vector3 newValue, Axis axis)
        {
            ref DynamicSpline splineAxis = ref GetSplineAxis(axis);
            int origin = splineAxis.QuantityPoints / 2;
            if (origin == index)
                return;

            splineAxis.UpdatePoint(newValue, index, true);
            UpdateTransPoint(index, newValue, axis);
        }

        private void UpdateTransPoint(int index, Vector3 newPoint, Axis axis)
        {
            if (axis == Axis.X)
                pointsAxisX[index].localPosition = newPoint;
            else
                pointsAxisY[index].localPosition = newPoint;
        }

        private void IncreaseResolutionSpline(Axis axis)
        {
            ref var splineAxis = ref GetSplineAxis(axis);
            var listPoints = axis == Axis.X ? pointsAxisX : pointsAxisY;
            splineAxis.IncreaseResolution(2);
            int origin = splineAxis.QuantityPoints / 2;
            splineAxis.UpdatePoint(Vector3.zero, origin, true);
            listPoints.Add(CreatePoint());
            listPoints.Add(CreatePoint());
            for (int i = 0; i < listPoints.Count; i++)
                listPoints[i].localPosition = splineAxis[i];
        }

        private void OnValidate()
        {
            if (typeAxisX != prevTypeAxisX)
            {
                UpdateTypeSpline(Axis.X, typeAxisX);
                UpdateLine(Axis.X);
                prevTypeAxisX = typeAxisX;
            }
            else if (typeAxisY != prevTypeAxisY)
            {
                UpdateTypeSpline(Axis.Y, typeAxisY);
                UpdateLine(Axis.Y);
                prevTypeAxisY = typeAxisY;
            }
            if (addPointToAxisX == true)
            {
                IncreaseResolutionSpline(Axis.X);
                addPointToAxisX = false;
            }
            else if (addPointToAxisY == true)
            {
                IncreaseResolutionSpline(Axis.Y);
                addPointToAxisY = false;
            }
            // // if (indexPointSelected >= spline.QuantityPoints || indexPointSelected < 0)
            // //     indexPointSelected = 0;

            // if (indexPointSelected != prevIndexSelected)
            // {
            //     positionPoint = pointsAxisX[indexPointSelected].localPosition;
            //     prevIndexSelected = indexPointSelected;
            // }
            // pointsAxisX[indexPointSelected].localPosition = positionPoint;
        }

        private void UpdateTypeSpline(Axis axis, TypeAxis typeAxis)
        {
            ref var currentAxis = ref GetSplineAxis(axis);
            int origin = currentAxis.QuantityPoints / 2;

            for (int i = 0; i < origin; i++)
                UpdatePoint(i, currentAxis[i], axis, typeAxis);
        }

        private void CreateAxis(Vector3 axis, List<Transform> axisList, out DynamicSpline spline)
        {
            var newPoints = new Vector3[5];
            for (int i = 0; i < 5; i++)
            {
                axisList.Add(CreatePoint());
                axisList[i].localPosition = newPoints[i] = (-axis * 2f) + (axis * i);
            }
            spline = new DynamicSpline(newPoints);
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

        private void UpdateLine(Axis axis)
        {

            switch (axis)
            {
                case Axis.X:
                    UpateDataLine(lineRendererX, in axisX);
                    break;
                case Axis.Y:
                    UpateDataLine(lineRendererY, in axisY);
                    break;
            }
        }

        private void UpateDataLine(LineRenderer line, in DynamicSpline axis)
        {
            line.positionCount = resolutionCurve;
            var posLine = new Vector3[resolutionCurve];
            float step = 1f / (resolutionCurve - 1f);

            for (int i = 0; i < resolutionCurve; i++)
                posLine[i] = axis.GetPoint(i * step) + Vector3.up * 0.05f;
            line.SetPositions(posLine);
        }

        private ref DynamicSpline GetSplineAxis(Axis axis)
        {
            if (axis == Axis.X)
                return ref axisX;
            else
                return ref axisY;
        }
    }

    public enum TypeAxis
    {
        Symetric, Asymetric, Free
    }

    public enum Axis
    {
        X, Y, Z
    }
}