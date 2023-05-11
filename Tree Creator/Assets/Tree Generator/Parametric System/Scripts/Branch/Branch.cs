using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    [System.Serializable]
    public class Branch : IBranch
    {

        #region Curve Parameters
        private List<Vector3> points;
        private int quantityPoints;

        private BranchData branchData;
        private TreeData treeData;
        private IBranch parent;
        private DynamicSpline mainSpline, secondarySpline;
        private BranchMesh branchMesh;
        private int levelBranch;

        private bool updateCurve;
        private bool updateChilds;
        private bool updateMesh;
        private bool updateOrientation;
        #endregion


        #region Orientation Data
        private Vector3 position;
        private Vector3 up, forward;
        private float turnAngle, pitchAngle, rollAngle;
        private float normalizedPosition;
        #endregion


        #region Branch Child Parameters
        private List<Branch> childs;
        private BranchData childData;
        private BranchManager branchManager;
        #endregion


        #region Data Initializer
        public Branch(TreeData treeData, int levelBranch, IBranch parent, BranchManager branchManager)
        {
            this.treeData = treeData;
            this.levelBranch = levelBranch;
            this.parent = parent;
            this.branchManager = branchManager;
            points = new List<Vector3>();
            this.branchData = treeData.GetBranchData(levelBranch);
            childs = new List<Branch>();
            this.childData = treeData.GetBranchData(levelBranch + 1);
            quantityPoints = branchData.NCurveRes + 1;
            forward = Vector3.forward;
            up = Vector3.up;
            mainSpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            secondarySpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            branchMesh = new BranchMesh(this, treeData, levelBranch);
            InitializePoints();
        }

        private void InitializePoints()
        {
            quantityPoints = branchData.NCurveRes + 1;
            for (int i = 0; i < quantityPoints; i++)
                points.Add(Vector3.zero);

            Update(true);
        }
        #endregion


        public void Update(bool parentUpdated = true)
        {
            // scaleChanged = false;
            updateCurve = parentUpdated == true || branchData.EqualBranchScale(GetCurrentData()) == false;
            updateMesh = updateCurve == true || treeData.EqualTrunkMeshStructure() == false ||
                    branchData.EqualMeshStructure(GetCurrentData()) == false;
            updateOrientation = branchData.EqualOrientation(GetCurrentData()) == false;

            if (treeData.CanHaveChilds(levelBranch))
                // updateChilds = updateCurve == true || branchData.EqualChilds(GetCurrentData()) == false;
                updateChilds = updateCurve == true ||
                    childData.EqualChilds(treeData.GetBranchData(levelBranch + 1)) == false;
            else
                updateChilds = false;



            if (updateCurve == true)
                UpdateBranchCurve();
            if(updateMesh == true)
                UpdateBranchMesh();

            // if (updateCurve == true && updateChilds == true)
            if (updateChilds == true)
                UpdateBranchChilds(updateCurve);
            // else if (updateCurve == false && updateChilds == true)
            //     UpdateBranchChilds(false);
            else if (treeData.CanHaveChilds(levelBranch) == false)
                RemoveChildsInUse();

            // if (branchData.Equals(GetCurrentData()) == false || parentUpdated)
            // {
            //     UpdateBranch();
            //     if (treeData.CanHaveChilds(levelBranch))
            //         UpdateBranchChilds();
            // }
            // else if (treeData.CanHaveChilds(levelBranch))
            // {
            //     if (childData.EqualChilds(treeData.GetBranchData(levelBranch + 1)) == false)
            //         UpdateBranchChilds();
            //     else
            //         UpdateBranchChilds(false);
            // }

            // if (treeData.CanHaveChilds(levelBranch) == false)
            //     RemoveChildsInUse();

            // else if (treeData.CanHaveChilds(levelBranch) &&
            //     childData.EqualsParent(treeData.GetBranchData(levelBranch + 1)) == false)
            //     UpdateBranchChilds();
            // else
            //     UpdateBranchChilds(false);
        }

        #region Update Branch Curve

        private void UpdateBranchCurve()
        {
            // scaleChanged = branchData.EqualScale(GetCurrentData()) == false;
            branchData = GetCurrentData();
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
            // var forward = Vector3.zero;
            var forward = Vector3.forward;
            var turtle = GetCurrentTurtle();
            // var turtle = new Turtle(forward, up);
            var angleRotation = GetAngleRotation(1);
            points[0] = position;
            // points[0] = Vector3.zero;
            // mainSpline.UpdatePoint(position, 1);

            turtle.Pitch(angleRotation);
            BiasTurtle(ref turtle);

            forward = turtle.GetForward();
            float stepPoints = GetLenght() / (float)(quantityPoints - 1);
            var currPoint = position;
            // var currPoint = Vector3.zero;

            for (int i = 1; i < quantityPoints; i++)
            {
                angleRotation = GetAngleRotation(i);
                turtle.Pitch(angleRotation);
                BiasTurtle(ref turtle);
                forward = turtle.GetForward();
                currPoint += forward * stepPoints;
                points[i] = currPoint;
            }
            UpdateMainSpline();
        }

        private void UpdateMainSpline()
        {
            mainSpline.UpdatePoint(points[0], 1);
            for (int i = 1; i < quantityPoints; i++)
                mainSpline.UpdatePoint(points[i], i + 1);

            // mainSpline.UpdatePoint(position, 1);
            // mainSpline.UpdatePoint(Vector3.zero, 1);
            var zeroPoint = points[0] + (points[0] - points[1]);
            mainSpline.UpdatePoint(zeroPoint, 0);
            var lastPoint = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);
            mainSpline.UpdatePoint(lastPoint, points.Count + 1);
            mainSpline.UpdateSpline();
            if (branchData.ShapeCurve == ShapeCurve.Spiral)
                UpdateSecondarySpline();
            // UpdateBranchMesh();
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

        private void UpdateBranchMesh()
        {
            if (branchData.ShapeCurve == ShapeCurve.Default)
                branchMesh.UpdateSpline(mainSpline);
            else
                branchMesh.UpdateSpline(secondarySpline);
            branchMesh.UpdateMesh();
        }


        #endregion


        #region Update Branch's Child Properties

        private void UpdateBranchChilds(bool parentModified = true)
        {
            if (parentModified == true)
            {
                // scaleChanged = true;
                childData = treeData.GetBranchData(levelBranch + 1);
                UpdateQuantityBranches();
            }
            else
                UpdateChildsWithoutParentChange();
        }

        private void UpdateQuantityBranches()
        {
            int stems_max = GetDesiredBranches();
            int k = childs.Count;
            for (int i = k; i < stems_max; i++)
            {
                if (k >= stems_max)
                    break;
                var newBranch = branchManager.GetBranch(levelBranch + 1, this);
                // childs.Add(new Branch(treeData, levelBranch + 1, this));
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
        }

        private void UpdatePropertiesChilds()
        {
            var sections = GetDesiredSections();
            for (int i = 0, n = 0; i < sections; i++)
            {
                if (branchData.ShapeCurve == ShapeCurve.Default)
                    UpdateBranchSection(n, i, childData.NBranchesPerSection, in mainSpline);
                else
                    UpdateBranchSection(n, i, childData.NBranchesPerSection, in secondarySpline);

                n += childData.NBranchesPerSection;
            }
        }

        private void UpdateBranchSection(int indexBranch, int indexSection, int branchesPerSection,
            in DynamicSpline spline)
        {
            var currentParentUp = GetCurrentTurtle().GetUp();
            var sections = GetDesiredSections();
            var stepPositionBranches = sections == 0 ? 0 : 1f / (float)(sections + 1);
            var currStep = (indexSection + 1) * stepPositionBranches;
            var currentPos = spline.GetPoint(currStep);
            var currentUp = spline.GetDerivative(currStep);
            var currentForward = spline.GetNormal(currStep, currentParentUp);

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
            stems_max = childData.NBranches;
            residuo = stems_max % childData.NBranchesPerSection;
            stems_max -= residuo;
            return stems_max;
        }

        private int GetDesiredSections()
        {
            return Mathf.RoundToInt(GetDesiredBranches() / childData.NBranchesPerSection);
        }

        private void UpdateChildsWithoutParentChange()
        {
            for (int i = 0; i < childs.Count; i++)
                childs[i].Update(false);
        }

        public void RemoveChildsInUse()
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

        private float GetAngleRotation(int indexPoint)
        {
            float angleRotation = 0;

            if (branchData.NCurveBack == 0)
                angleRotation = branchData.NCurve / (float)branchData.NCurveRes;
            else
            {
                if (indexPoint < Mathf.RoundToInt(branchData.NCurveRes / 2))
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

        private Turtle GetCurrentTurtle()
        {
            var turtle = new Turtle(forward, up);
            turtle.Turn(turnAngle);
            turtle.Pitch(pitchAngle);
            turtle.Roll(rollAngle);
            return turtle;
        }

        private BranchData GetCurrentData()
        {
            return treeData.GetBranchData(levelBranch);
        }

        #endregion


        #region Public API

        public Vector3[] GetPoints() => points.ToArray();
        public Branch[] GetChilds() => childs != null ? childs.ToArray() : null;
        public int GetLevelBranch() => levelBranch;

        public void UpdateCoreData(int levelBranch, IBranch parent)
        {
            this.levelBranch = levelBranch;
            this.parent = parent;
        }

        public float GetLenght()
        {
            float lenghtBranch = 0f;
            float lenght_child_max = branchData.NLength + branchData.NLengthV;
            float offset = parent.GetOffset(normalizedPosition);
            if (levelBranch == 1)
                lenghtBranch = parent.GetLenght() * lenght_child_max *
                    TreeUtils.ShapeRatio(treeData.Shape, normalizedPosition);
            else
                lenghtBranch = lenght_child_max * (parent.GetLenght() - 0.6f * offset);

            return lenghtBranch;
        }

        public void Turn(float angle)
        {
            turnAngle = angle;
        }

        public void Roll(float angle)
        {
            rollAngle = angle;
        }

        public void Pitch(float angle)
        {
            pitchAngle = angle;
        }

        public void UpdateUp(Vector3 up)
        {
            this.up = up;
        }

        public void UpdateForward(Vector3 forward)
        {
            this.forward = forward;
        }

        public void UpdateAxes(Vector3 position, Vector3 forward, Vector3 up)
        {
            this.position = position;
            this.forward = forward;
            this.up = up;
        }

        public void UpdatePosition(Vector3 newPosition)
        {
            position = newPosition;
        }

        public void UpdateNormalizePosition(float newPos)
        {
            newPos = Mathf.Clamp01(newPos);
            normalizedPosition = newPos;
        }

        public float GetOffset(float normalizePosition)
        {
            normalizePosition = Mathf.Clamp01(normalizePosition);
            return mainSpline.GetLenghtAt(normalizePosition);
        }

        public Mesh GetBranchMesh()
        {
            if (branchData.ShapeCurve == ShapeCurve.Default)
                branchMesh.UpdateSpline(mainSpline);
            else
                branchMesh.UpdateSpline(secondarySpline);
            branchMesh.UpdateMesh();
            return branchMesh.GetMesh();
        }

        public float GetRadiusBase()
        {
            return parent.GetRadiusBase() * Mathf.Pow((GetLenght() / parent.GetLenght()), treeData.RatioPower);
        }

        public Matrix4x4 GetCurrentOrientation()
        {
            var orientation = new Matrix4x4();
            var currRotation = Quaternion.LookRotation(forward, up);
            orientation.SetTRS(position, currRotation, Vector3.one);
            return orientation;
        }

        public Turtle GetOrientationBranch()
        {
            var currentTurtle = GetCurrentTurtle();
            currentTurtle.SetPosition(position);
            return currentTurtle;
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