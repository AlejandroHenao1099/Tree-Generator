using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class BranchNode
    {
        private List<Vector3> points;
        private int quantityPoints;

        private BranchData branchData;
        private TreeData treeData;
        private IBranchRead parent;
        private DynamicSpline mainSpline, secondarySpline;
        private int levelBranch;


        private Vector3 position;
        private Vector3 up;
        private float turnAngle, pitchAngle, rollAngle;
        private float normalizedPosition;
        private float prevNormalizedPosition;


        private void UpdateBranchCurve()
        {
            UpdateQuantityPointsCurve();
        }

        private void UpdateQuantityPointsCurve()
        {
            quantityPoints = branchData.NCurveRes + 1;
            int currLenght = points.Count;
            if (currLenght > quantityPoints)
            {
                int diff = currLenght - quantityPoints;
                for (int i = 0; i < diff; i++)
                {
                    points.RemoveAt(points.Count - 1);
                    mainSpline.RemovePoint(mainSpline.QuantityPoints - 1);
                    secondarySpline.RemovePoint(secondarySpline.QuantityPoints - 1);
                }
            }
            else
            {
                for (int i = currLenght; i < quantityPoints; i++)
                {
                    points.Add(Vector3.zero);
                    mainSpline.AddPoint(Vector3.zero);
                    secondarySpline.AddPoint(Vector3.zero);
                }
            }
            UpdatePoints();
        }

        private void UpdatePoints()
        {
            var curveForward = Vector3.forward;
            var turtle = new Turtle(Vector3.forward, Vector3.up);
            var angleRotation = GetAngleRotation(0);
            points[0] = Vector3.zero;

            turtle.Pitch(angleRotation);
            BiasTurtle(ref turtle);

            curveForward = turtle.GetForward();
            float stepPoints = GetLenght() / (float)(quantityPoints - 1);
            var currPoint = Vector3.zero;

            for (int i = 1; i < quantityPoints; i++)
            {
                angleRotation = GetAngleRotation(i);
                turtle.Pitch(angleRotation);
                BiasTurtle(ref turtle);
                curveForward = turtle.GetForward();
                currPoint += curveForward * stepPoints;
                points[i] = currPoint;
            }
            UpdateMainSpline();
        }

        private void UpdateMainSpline()
        {
            mainSpline.UpdatePoint(points[0], 1);
            for (int i = 1; i < quantityPoints; i++)
                mainSpline.UpdatePoint(points[i], i + 1);

            var zeroPoint = points[0] + (points[0] - points[1]);
            mainSpline.UpdatePoint(zeroPoint, 0);
            var lastPoint = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);
            mainSpline.UpdatePoint(lastPoint, points.Count + 1);
            mainSpline.UpdateSpline();
            if (branchData.ShapeCurve == ShapeCurve.Spiral)
                UpdateSecondarySpline();
        }

        private void UpdateSecondarySpline()
        {
            var stepCurve = 1f / (quantityPoints - 1);
            var radiusDifference = Mathf.Abs(branchData.NRadiusBase - branchData.NRadiusTop);
            var stepRadius = radiusDifference / (quantityPoints - 1);
            stepRadius = branchData.NRadiusBase <= branchData.NRadiusTop ? stepRadius : -stepRadius;
            secondarySpline.UpdatePoint(points[0], 1);
            for (int i = 1; i < quantityPoints; i++)
            {
                float currRadius = branchData.NRadiusBase + (stepRadius * i);
                float anguloRotacion = (branchData.NRotateSpiral + branchData.NRotateSpiralV) * i;
                var currPoint = mainSpline.GetCylindricalCoordinates(stepCurve * i, anguloRotacion, up, currRadius);
                secondarySpline.UpdatePoint(currPoint, i + 1);
            }
            var zeroPoint = points[0] + (points[0] - points[1]);
            secondarySpline.UpdatePoint(zeroPoint, 0);
            var lastPoint = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);
            secondarySpline.UpdatePoint(lastPoint, points.Count + 1);
            secondarySpline.UpdateSpline();
        }
    }
}