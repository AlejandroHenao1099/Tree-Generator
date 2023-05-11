using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    public class TrunkMono : MonoBehaviour
    {

        public bool showSpline = false;
        public float sizeSube = 0.1f;
        public int resolutionSpline = 10;

        public TreeData treeData;
        public TrunkNode trunkNode;
        public BranchNodeManager branchManager;


        private bool onStart;
        DynamicSpline currentSpline;

        private void Awake()
        {
            branchManager.SetTreeData(treeData);
            trunkNode = new TrunkNode(treeData, branchManager);
            GetComponent<MeshFilter>().mesh = trunkNode.GetTrunkMesh();
            onStart = true;
        }

        private void OnValidate()
        {
            if (onStart == false)
                return;

            if (treeData.EqualLevels() == false)
            {
                int currentLevel = treeData.Levels;
                trunkNode.Update(false);
            }
            else
            {
                int levelOfUpdate = treeData.UpdateAtLevel();
                bool treeUpdate = treeData.EqualsTreeScale() == false;
                if (levelOfUpdate == 0)
                    trunkNode.Update(treeUpdate);
                else
                    branchManager.UpdateBranches(levelOfUpdate);
                if (showSpline == true)
                    trunkNode.GetSplineinUse(out currentSpline);
            }

            treeData.UpdateCaching();
        }

        private void OnDrawGizmos()
        {
            if (showSpline == false)
                return;
            float step = 1f / (resolutionSpline - 1);
            for (int i = 0; i < resolutionSpline; i++)
            {
                Vector3 currPos = currentSpline.GetPoint(i * step);
                Gizmos.DrawCube(currPos, Vector3.one * sizeSube);
            }
        }

    }
}