using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    [System.Serializable]
    public class Trunk : IBranch
    {

        #region Curve Parameters
        private List<Vector3> points;
        private int quantityPoints;
        private TrunkData trunkData;
        private TreeData treeData;
        private DynamicSpline mainSpline, secondarySpline;
        private TrunkMesh trunkMesh;

        private bool updateCurve;
        private bool updateChilds;
        private bool updateMesh;
        #endregion


        #region Branch Child Parameters
        private List<Branch> childs;
        private BranchData childData;
        private BranchManager branchManager;
        #endregion


        #region Data Initializer

        public Trunk(TreeData treeData, BranchManager branchManager)
        {
            this.treeData = treeData;
            this.branchManager = branchManager;
            this.trunkData = treeData.GetTrunkData();
            this.childData = treeData.GetBranchData(1);
            points = new List<Vector3>();
            childs = new List<Branch>();
            quantityPoints = trunkData.NCurveRes + 1;
            mainSpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            secondarySpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            trunkMesh = new TrunkMesh(mainSpline, treeData);
            InitializePoints();
        }

        private void InitializePoints()
        {
            quantityPoints = trunkData.NCurveRes + 1;
            for (int i = 0; i < quantityPoints; i++)
                points.Add(Vector3.zero);

            Update(true);
        }
        #endregion


        public void Update(bool updatedTreeData = false)
        {
            // scaleChanged = false;

            updateCurve = updatedTreeData == true ||
                    trunkData.EqualTrunkScale(treeData.GetTrunkData()) == false;
            updateMesh = updateCurve == true || treeData.EqualTrunkMeshStructure() == false ||
                    trunkData.EqualMeshStructure(treeData.GetTrunkData()) == false;

            if (treeData.CanHaveChilds(0))
                updateChilds = updateCurve == true ||
                    trunkData.EqualChilds(treeData.GetTrunkData()) == false ||
                        childData.EqualChilds(treeData.GetBranchData(1)) == false;
            else
                updateChilds = false;


            if (updateCurve == true)
                UpdateBranchCurve();
            if (updateMesh == true)
                UpdateBranchMesh();

            if (updateChilds == true)
                UpdateBranchChilds(updateCurve);
            else if (treeData.CanHaveChilds(0) == false)
                RemoveChildsInUse();
            else
                UpdateChildsWithoutParentChange();


            // if (trunkData.Equals(treeData.GetTrunkData()) == false || updatedTreeData == true)
            // {
            //     UpdateBranch();
            //     if (treeData.CanHaveChilds(0))
            //     {
            //         UpdateBranchChilds();
            //     }
            // }
            // else if (treeData.CanHaveChilds(0))
            // {
            //     if (childData.EqualChilds(treeData.GetBranchData(1)) == false)
            //     {
            //         UpdateBranchChilds();

            //     }
            //     else
            //     {
            //         UpdateBranchChilds(false);
            //     }
            // }

            // if (treeData.CanHaveChilds(0) == false)
            // {
            //     RemoveChildsInUse();
            // }
        }


        #region  Update Trunk Curve

        private void UpdateBranchCurve()
        {
            trunkData = treeData.GetTrunkData();
            UpdateQuantityPoints();
        }

        private void UpdateQuantityPoints()
        {
            quantityPoints = trunkData.NCurveRes + 1;
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
            var angleRotation = GetAngleRotation(1);
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
            // UpdatePropertiesChilds();
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

            // UpdateBranchMesh();
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

        private void UpdateBranchMesh()
        {
            if (trunkData.ShapeCurve == ShapeCurve.Default)
                trunkMesh.UpdateSpline(mainSpline);
            else
                trunkMesh.UpdateSpline(secondarySpline);
            trunkMesh.UpdateMesh();
        }

        #endregion


        #region Update Trunk's Child Properties

        private void UpdateBranchChilds(bool parentModified = true)
        {
            if (parentModified == true || updateChilds == true)
            {
                // scaleChanged = true;
                childData = treeData.GetBranchData(1);
                UpdateQuantityBranches();
            }
            // else
            // UpdateChildsWithoutParentChange();

        }

        private void UpdateQuantityBranches()
        {
            int stems_max = GetDesiredBranches();
            int k = childs.Count;
            for (int i = k; i < stems_max; i++)
            {
                if (k >= stems_max)
                    break;
                var newBranch = branchManager.GetBranch(1, this);
                childs.Add(newBranch);
                k++;
            }

            int currentChilds = childs.Count;
            if (currentChilds > stems_max)
                for (int i = currentChilds; i != stems_max; i--)
                {
                    var childToRemove = childs[childs.Count - 1];
                    childs.RemoveAt(childs.Count - 1);
                    branchManager.RemoveBranch(childToRemove);
                }
            UpdatePropertiesChilds();


            // int segSplits_effective = 0;
            // if (treeData.trunkData.nSegSplits > 0)
            // {
            //     float Error_Value = treeData.GetError(0);
            //     for (int i = 1; i < quantityPoints - 2; i++)
            //     {
            //         int currentEffective = Mathf.RoundToInt(trunkData.nSegSplits + Error_Value);
            //         segSplits_effective += currentEffective;

            //         Error_Value -= ((float)segSplits_effective - trunkData.nSegSplits);
            //     }
            // }
            // return segSplits_effective;
        }

        private void UpdatePropertiesChilds()
        {
            var sections = GetDesiredSections();
            var n = 0;
            int i = 0;
            if (trunkData.NBaseSplits > 0 && trunkData.NBaseSplits <= childData.NBranches)
            {
                if (trunkData.ShapeCurve == ShapeCurve.Default)
                    UpdateBranchSection(n, 0, trunkData.NBaseSplits, in mainSpline);
                else
                    UpdateBranchSection(n, 0, trunkData.NBaseSplits, in secondarySpline);

                n += trunkData.NBaseSplits;
                i++;
            }
            for (; i < sections; i++)
            {
                if (trunkData.ShapeCurve == ShapeCurve.Default)
                    UpdateBranchSection(n, i, childData.NBranchesPerSection, in mainSpline);
                else
                    UpdateBranchSection(n, i, childData.NBranchesPerSection, in secondarySpline);

                n += childData.NBranchesPerSection;
            }
        }

        private void UpdateBranchSection(int indexBranch, int indexSection, int branchesPerSection,
            in DynamicSpline spline)
        {
            var currentOrientationUp = GetTrunkUp();
            var sections = GetDesiredSections();
            var stepPositionBranches = sections == 1 ? 0f : (1f - treeData.BaseSize) / (float)(sections);
            var currStep = (indexSection * stepPositionBranches) + treeData.BaseSize;
            var currentPos = spline.GetPoint(currStep);
            var currentUp = spline.GetDerivative(currStep);
            var currentForward = spline.GetNormal(currStep, currentOrientationUp);
            for (int j = 0; j < branchesPerSection; j++)
            {
                childs[indexBranch].UpdateAxes(currentPos, currentForward, currentUp);
                childs[indexBranch].UpdateNormalizePosition(currStep);
                float anguloRotacion = 0f;
                if (childData.NRotate >= 0)
                    anguloRotacion = (childData.NRotate + childData.NRotateV) * (indexBranch + 1);
                else
                    anguloRotacion = (180 + childData.NRotate + childData.NRotateV) * (j + 1);
                childs[indexBranch].Turn(anguloRotacion);

                if (childData.NDownAngleV >= 0f)
                    anguloRotacion = childData.NDownAngle + childData.NDownAngleV;
                else
                    anguloRotacion = childData.NDownAngle +
                        (childData.NDownAngleV * (1 - 2 *
                            TreeUtils.ShapeRatio(treeData.Shape, currStep)));
                childs[indexBranch].Pitch(90 - anguloRotacion);

                childs[indexBranch++].Update(updateCurve);
            }
        }

        private int GetDesiredBranches()
        {
            int stems_max = 0;
            int residuo = 0;
            if (trunkData.NBaseSplits > 1 && trunkData.NBaseSplits < childData.NBranches)
            {
                stems_max = childData.NBranches - trunkData.NBaseSplits;
                residuo = stems_max % childData.NBranchesPerSection;
                stems_max -= residuo;
                return stems_max + trunkData.NBaseSplits;
            }
            else if (trunkData.NBaseSplits == childData.NBranches)
                return trunkData.NBaseSplits;

            stems_max = childData.NBranches;
            residuo = stems_max % childData.NBranchesPerSection;
            stems_max -= residuo;
            return stems_max;
        }

        private int GetDesiredSections()
        {
            int stems_max = 0;
            if (trunkData.NBaseSplits > 1 && trunkData.NBaseSplits <= childData.NBranches)
            {
                stems_max = GetDesiredBranches() - trunkData.NBaseSplits;
                stems_max = Mathf.RoundToInt(stems_max / childData.NBranchesPerSection);
                return stems_max + 1;
            }
            return Mathf.RoundToInt(GetDesiredBranches() / childData.NBranchesPerSection);
        }

        private void UpdateChildsWithoutParentChange()
        {
            for (int i = 0; i < childs.Count; i++)
                childs[i].Update(false);
        }

        private void RemoveChildsInUse()
        {
            if (childs.Count <= 0) return;

            int currentChilds = childs.Count;
            for (int i = currentChilds; i > 0; i--)
            {
                var childToRemove = childs[childs.Count - 1];
                childs.RemoveAt(childs.Count - 1);
                branchManager.RemoveBranch(childToRemove);
            }
        }

        #endregion


        #region Utils

        private int GetAngleRotation(int indexPoint)
        {
            int angleRotation = 0;

            if (trunkData.NCurveBack == 0)
                angleRotation = trunkData.NCurve / trunkData.NCurveRes;
            else
            {
                if (indexPoint < Mathf.RoundToInt(trunkData.NCurveRes / 2))
                    angleRotation = trunkData.NCurve / trunkData.NCurveRes / 2;
                else
                    angleRotation = trunkData.NCurveBack / trunkData.NCurveRes / 2;

                angleRotation += (trunkData.NCurveV / trunkData.NCurveRes) * TreeUtils.RandomSign();
            }
            return angleRotation;
        }

        private Matrix4x4 GetCurrentOrientation()
        {
            var currentOrientation = new Matrix4x4();
            currentOrientation.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            var currentRotation = currentOrientation.rotation;
            // var newRotation = Quaternion.Euler(0, -trunkData.angleTurn, 0);
            var currentPosition = currentOrientation.MultiplyPoint3x4(Vector3.zero);
            currentOrientation.SetTRS(currentPosition, currentRotation, Vector3.one);
            return currentOrientation;
        }

        private Vector3 GetTrunkUp() => GetCurrentOrientation().MultiplyVector(Vector3.forward);

        private Vector3 GetTrunkForward() => GetCurrentOrientation().MultiplyVector(Vector3.up);
        #endregion


        #region Public API

        public Vector3[] GetPoints() => points.ToArray();
        public Branch[] GetChilds() => childs.ToArray();

        public void GetAxis(out Vector3 forward, out Vector3 up, out Vector3 right)
        {
            var currOr = GetCurrentOrientation();
            forward = currOr.MultiplyVector(Vector3.forward);
            right = currOr.MultiplyVector(Vector3.right);
            up = currOr.MultiplyVector(Vector3.up);
        }

        public float GetLenght()
        {
            float scaleTree = treeData.GetScaleTree();
            return (trunkData.NLength + trunkData.NLengthV) * scaleTree;
        }

        public float GetOffset(float normalizePosition)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            return mainSpline.GetLenghtAt(normalizePosition);
        }

        public void GetSplineinUse(out DynamicSpline spline)
        {
            if (trunkData.ShapeCurve == ShapeCurve.Default)
            {
                spline = mainSpline;
                return;
            }
            spline = secondarySpline;
        }

        public Mesh GetTrunkMesh()
        {
            if (trunkData.ShapeCurve == ShapeCurve.Default)
                trunkMesh.UpdateSpline(mainSpline);
            else
                trunkMesh.UpdateSpline(secondarySpline);
            trunkMesh.UpdateMesh();
            return trunkMesh.GetMesh();
        }

        public float GetRadiusBase()
        {
            return GetLenght() * treeData.Ratio * trunkData.NScale;
        }

        public Vector3 GetPositionOnSurface(float t, float angle)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetNormalOnSurface(float t, float angle)
        {
            throw new System.NotImplementedException();
        }

        public Vector3 GetBitangentOnSurface(float t)
        {
            throw new System.NotImplementedException();
        }

        public void GetPNBOnSurface(float t, float angle, out Vector3 position, out Vector3 normal, out Vector3 bitangent)
        {
            throw new System.NotImplementedException();
        }

        public void GetPNOnSurface(float t, float angle, out Vector3 position, out Vector3 normal)
        {
            throw new System.NotImplementedException();
        }

        #endregion

    }
}