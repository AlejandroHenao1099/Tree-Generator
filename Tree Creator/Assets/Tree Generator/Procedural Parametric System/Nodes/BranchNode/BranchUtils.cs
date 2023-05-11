using UnityEngine;

namespace TreeCreator
{
    public partial class BranchNode
    {
        private float GetAngleRotation(int indexPoint)
        {
            float angleRotation = 0;

            if (branchData.NCurveBack == 0)
                angleRotation = branchData.NCurve / (float)branchData.NCurveRes;
            else
            {
                if (indexPoint < Mathf.FloorToInt(branchData.NCurveRes / 2))
                    angleRotation = branchData.NCurve / (float)branchData.NCurveRes / 2f;
                else
                    angleRotation = branchData.NCurveBack / (float)branchData.NCurveRes / 2f;

                angleRotation += (branchData.NCurveV / branchData.NCurveRes) * TreeUtils.RandomSign();
            }
            return angleRotation;
        }

        private void BiasTurtle(ref Turtle turtle)
        {
            if (levelBranch >= 2)
            {
                var turtleZ = turtle.GetForward();
                var turtleY = turtle.GetUp();
                var attractionUp = treeData.AttractionUp;

                float declination = Mathf.Acos(turtleZ.z);
                float orientation = Mathf.Acos(turtleY.z);
                float curve_up_segment = attractionUp * declination *
                    Mathf.Cos(orientation) / branchData.NCurveRes;
                if (attractionUp < 0)
                    turtle.Bias(Vector3.down, Mathf.Abs(curve_up_segment));
                else
                    turtle.Bias(Vector3.up, Mathf.Abs(curve_up_segment));
            }
        }

        private bool EqualNormalizePosition()
        {
            return normalizedPosition.Equals(prevNormalizedPosition);
        }

        private void CachingNormalizePosition() => prevNormalizedPosition = normalizedPosition;

        private BranchData GetCurrentData()
        {
            return treeData.GetBranchData(levelBranch);
        }

        private float GetTurnAngle()
        {
            float anguloRotacion = 0f;
            if (branchData.NRotate >= 0)
                anguloRotacion = (branchData.NRotate + branchData.NRotateV) * indexBranch;
            else
                anguloRotacion = (180 + branchData.NRotate + branchData.NRotateV) * localIndex;
            return anguloRotacion;
        }

        private float GetPitchAngle()
        {
            float anguloRotacion = 0;
            if (branchData.NDownAngleV >= 0f)
                anguloRotacion = branchData.NDownAngle + branchData.NDownAngleV;
            else
                anguloRotacion = branchData.NDownAngle +
                    (branchData.NDownAngleV * (1 - 2 *
                        TreeUtils.ShapeRatio(treeData.Shape, normalizedPosition)));
            return 90 - anguloRotacion;
        }

        private void RestartControlVariables()
        {
            scaleUpdated = curveUpdated = numberChildsUpdated = orientationUpdated = false;
            updateCurve = updateChilds = updateMesh = updateOrientation = false;
        }
    }
}