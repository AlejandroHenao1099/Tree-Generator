using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class TrunkNode
    {
        private List<Vector3> points;
        private int quantityPoints;
        private TrunkData trunkData;
        private TreeData treeData;
        private DynamicSpline mainSpline, secondarySpline;
        // private TrunkMesh trunkMesh;

        private bool updateCurve;
        private bool updateChilds;
        private bool updateMesh;

        private void UpdateBranchCurve()
        {
            trunkData = treeData.GetTrunkData();
            UpdateQuantityPoints();
        }

        private void UpdateQuantityPoints()
        {
            quantityPoints = GetCurveRes() + 1;
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
            var currentOrientation = GetCurrentOrientation();
            var forward = Vector3.zero;
            // var angleRotation = GetAngleRotation(1);
            var angleRotation = GetAngleRotation(0);
            var turtle = new Turtle(currentOrientation.MultiplyVector(Vector3.up), currentOrientation.MultiplyVector(Vector3.forward));

            turtle.Pitch(angleRotation);
            forward = turtle.GetForward();
            float stepPoints = GetLenght() / (float)(quantityPoints - 1);
            var currPoint = Vector3.zero;

            for (int i = 1; i < quantityPoints; i++)
            {
                angleRotation = GetAngleRotation(i);
                turtle.Pitch(angleRotation);
                forward = turtle.GetForward();
                currPoint += forward * stepPoints;
                points[i] = currPoint;
            }
            UpdateMainSpline();
        }

        private void UpdateMainSpline()
        {
            for (int i = 1; i < quantityPoints; i++)
                mainSpline.UpdatePoint(points[i], i + 1);

            var zeroPoint = points[0] + (points[0] - points[1]);
            mainSpline.UpdatePoint(zeroPoint, 0);
            var lastPoint = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);
            mainSpline.UpdatePoint(lastPoint, points.Count + 1);
            mainSpline.UpdateSpline();
            if (trunkData.ShapeCurve == ShapeCurve.Spiral)
                UpdateSecondarySpline();
        }

        private void UpdateSecondarySpline()
        {
            var stepCurve = 1f / (quantityPoints - 1);
            var radiusDifference = Mathf.Abs(trunkData.NRadiusBase - trunkData.NRadiusTop);
            var stepRadius = radiusDifference / (quantityPoints - 1);
            stepRadius = trunkData.NRadiusBase <= trunkData.NRadiusTop ? stepRadius : -stepRadius;
            for (int i = 0; i < quantityPoints; i++)
            {
                float currRadius = trunkData.NRadiusBase + (stepRadius * i);
                float anguloRotacion = (trunkData.NRotateSpiral + trunkData.NRotateSpiralV) * i;
                var currPoint = mainSpline.GetCylindricalCoordinates(stepCurve * i, anguloRotacion, GetTrunkUp(), currRadius);
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