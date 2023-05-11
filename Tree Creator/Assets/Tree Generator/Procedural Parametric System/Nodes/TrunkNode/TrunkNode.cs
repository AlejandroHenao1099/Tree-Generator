using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class TrunkNode : IBranchRead
    {

        public TrunkNode(TreeData treeData, BranchNodeManager branchManager)
        {
            this.treeData = treeData;
            this.branchManager = branchManager;
            this.trunkData = treeData.GetTrunkData();
            points = new List<Vector3>();
            childs = new List<IBranchWrite>();
            quantityPoints = GetCurveRes() + 1;
            mainSpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            secondarySpline = new DynamicSpline(new Vector3[quantityPoints + 2]);
            InitializeMesh();
            InitializePoints();
            // 0316 561 8930  Ahorro a la mano Beatriz Arteaga
        }

        private void InitializePoints()
        {
            quantityPoints = GetCurveRes() + 1;
            for (int i = 0; i < quantityPoints; i++)
                points.Add(Vector3.zero);

            Update(true);
        }

        public void Update(bool updatedTreeData = false)
        {
            updateCurve = MustUpdateCurve(updatedTreeData);
            updateMesh = MustUpdateMesh(updatedTreeData);

            if (treeData.CanHaveChilds(0))
                updateChilds = MustUpdateChilds(updatedTreeData);
            else
                updateChilds = false;

            // Debug.Log("Curve: " + updateCurve + " Mesh: " + updateMesh + " TreeData: " + updatedTreeData + 
            //     " Childs: " + updateChilds);
            
            trunkData = treeData.GetTrunkData();

            if (updateCurve == true)
                UpdateBranchCurve();
            if (updateMesh == true)
                UpdateBranchMesh();

            if (updateChilds == true)
                UpdateBranchChilds();
            else if (treeData.CanHaveChilds(0) == false)
                RemoveChildsInUse();
            else
                UpdateChildsWithoutParentChange();

            RestartControlVariables();
        }


        private bool MustUpdateCurve(bool updatedTreeData)
        {
            var newTrunkData = treeData.GetTrunkData();
            scaleUpdated = trunkData.EqualTrunkScale(newTrunkData) == false;
            curveUpdated = trunkData.EqualCurveStructure(newTrunkData) == false;

            return treeData.EqualsTreeScale() == false ||
                    scaleUpdated == true || curveUpdated == true;
        }

        private bool MustUpdateMesh(bool updatedTreeData)
        {
            return updateCurve == true || treeData.EqualTrunkMeshStructure() == false ||
                    trunkData.EqualMeshStructure(treeData.GetTrunkData()) == false;
        }

        private bool MustUpdateChilds(bool updatedTreeData)
        {
            var newTrunkData = treeData.GetTrunkData();
            numberChildsUpdated = childs.Count != newTrunkData.NBranches ||
                    trunkData.EqualChilds(newTrunkData) == false;

            return updateCurve == true || treeData.EqualsTreeScale() == false || 
                    numberChildsUpdated == true ||
                        trunkData.EqualsScale(newTrunkData) == false ||
                            treeData.EqualChildDistribution() == false;
        }
        
    }
}