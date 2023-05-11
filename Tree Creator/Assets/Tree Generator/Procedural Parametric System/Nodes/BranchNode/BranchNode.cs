using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class BranchNode : IBranchRead, IBranchWrite
    {
        private bool updateCurve;
        private bool updateChilds;
        private bool updateMesh;
        private bool updateOrientation;

        private int indexBranch;
        private int localIndex;


        public BranchNode(TreeData treeData, int levelBranch, IBranchRead parent, BranchNodeManager branchManager)
        {
            this.treeData = treeData;
            this.levelBranch = levelBranch;
            this.parent = parent;
            this.branchManager = branchManager;
            points = new List<Vector3>();
            this.branchData = new BranchData();
            childs = new List<IBranchWrite>();
            quantityPoints = branchData.NCurveRes + 1;
            up = Vector3.up;
            mainSpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            secondarySpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            InitializeMesh();
            InitializeObject();
            InitializePoints();
        }

        private void InitializePoints()
        {
            quantityPoints = branchData.NCurveRes + 1;
            for (int i = 0; i < quantityPoints; i++)
                points.Add(Vector3.zero);
        }


        public void Update(bool parentScaleUpdated, bool parentCurveUpdated,
            bool parentNumberChildsUpdated, bool parentUpdatedOrientation, bool parentMeshUpdated)
        {
            updateCurve = MustUpdateCurve(parentScaleUpdated, parentNumberChildsUpdated);
            updateMesh = MustUpdateMesh();
            updateOrientation = MustUpdateOrientation(parentScaleUpdated, parentCurveUpdated,
                parentNumberChildsUpdated, parentUpdatedOrientation, parentMeshUpdated);

            if (treeData.CanHaveChilds(levelBranch))
                updateChilds = MustUpdateChilds(parentScaleUpdated, parentCurveUpdated);
            else
                updateChilds = false;

            // Debug.Log("Parent Scale: " + parentScaleUpdated + "  Parent Mesh: " + parentMeshUpdated + 
            //     "  Parent Orientation: " + parentUpdatedOrientation + "  Parent Curve: " + parentCurveUpdated
            //         + "   Parent Number Childs: " + parentNumberChildsUpdated);

            // Debug.Log("Curve: " + updateCurve + "   Mesh: " + updateMesh + "   Orientation: " + updateOrientation
            //     + "   Childs: " + updateChilds);

            branchData = GetCurrentData();

            if (updateCurve == true)
                UpdateBranchCurve();
            if (updateMesh == true)
                UpdateBranchMesh();
            if (updateOrientation == true)
                UpdateOrientation();

            if (updateChilds == true)
                UpdateBranchChilds();
            else if (treeData.CanHaveChilds(levelBranch) == false)
                RemoveChildsInUse();
            else
                UpdateChildsWithoutParentChange();

            CachingNormalizePosition();
            RestartControlVariables();
        }

        private bool MustUpdateCurve(bool parentScaleUpdated, bool parentNumberChildsUpdated)
        {
            scaleUpdated = parentScaleUpdated == true || branchData.EqualBranchScale(GetCurrentData()) == false
                || EqualNormalizePosition() == false;
            curveUpdated = branchData.EqualCurveStructure(GetCurrentData()) == false;

            return scaleUpdated == true || curveUpdated == true || parentNumberChildsUpdated == true
                        || treeData.EqualChildDistribution() == false;
        }

        private bool MustUpdateMesh()
        {
            return updateCurve == true || treeData.EqualBranchMeshStructure() == false ||
                    branchData.EqualMeshStructure(GetCurrentData()) == false;
        }

        private bool MustUpdateOrientation(bool parentScaleUpdated, bool parentCurveUpdated,
            bool parentNumberChildsUpdated, bool parentUpdatedOrientation,
                bool parentMeshUpdated)
        {
            orientationUpdated = parentNumberChildsUpdated == true || parentScaleUpdated == true ||
                parentCurveUpdated == true || parentUpdatedOrientation == true || parentMeshUpdated == true ||
                    branchData.EqualOrientation(GetCurrentData()) == false
                        || treeData.EqualChildDistribution() == false || EqualNormalizePosition() == false
                            || EqualNormalizePosition() == false;
            return orientationUpdated;
        }

        private bool MustUpdateChilds(bool parentScaleUpdated, bool parentCurveUpdated)
        {
            numberChildsUpdated = childs.Count != GetCurrentData().NBranches ||
                branchData.EqualChilds(GetCurrentData()) == false;
            return updateCurve == true || parentScaleUpdated == true || parentCurveUpdated == true ||
                numberChildsUpdated == true || treeData.EqualsTreeScale() == false;
        }
    }
}