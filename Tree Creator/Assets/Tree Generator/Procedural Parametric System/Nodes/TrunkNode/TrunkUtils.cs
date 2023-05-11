using UnityEngine;

namespace TreeCreator
{
    public partial class TrunkNode
    {
        private int GetAngleRotation(int indexPoint)
        {
            int angleRotation = 0;

            if (trunkData.NCurveBack == 0)
                angleRotation = trunkData.NCurve / GetCurveRes();
            else
            {
                if (indexPoint < Mathf.RoundToInt(GetCurveRes() / 2))
                    angleRotation = trunkData.NCurve / GetCurveRes() / 2;
                else
                    angleRotation = trunkData.NCurveBack / GetCurveRes() / 2;

                angleRotation += (trunkData.NCurveV / GetCurveRes()) * TreeUtils.RandomSign();
            }
            return angleRotation;
        }

        private Matrix4x4 GetCurrentOrientation()
        {
            var currentOrientation = new Matrix4x4();
            currentOrientation.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            var currentRotation = currentOrientation.rotation;
            var currentPosition = currentOrientation.MultiplyPoint3x4(Vector3.zero);
            currentOrientation.SetTRS(currentPosition, currentRotation, Vector3.one);
            return currentOrientation;
        }

        private Vector3 GetTrunkUp() => GetCurrentOrientation().MultiplyVector(Vector3.forward);

        private Vector3 GetTrunkForward() => GetCurrentOrientation().MultiplyVector(Vector3.up);

        private int GetCurveRes() => trunkData.NCurveRes;

        private int GetBranchesPerSection() => trunkData.NBranchesPerSection;

        private int GetResolutionVertical() => trunkData.ResolutionVertical;
        private int GetResolutionHorizontal() => trunkData.ResolutionHorizontal;

        private void RestartControlVariables()
        {
            scaleUpdated = curveUpdated = numberChildsUpdated = false;
            updateCurve = updateChilds = updateMesh = false;
        }
    }
}