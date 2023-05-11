using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class BranchNode
    {
        private List<IBranchWrite> childs;
        private BranchNodeManager branchManager;
        private bool scaleUpdated, curveUpdated, numberChildsUpdated, orientationUpdated;


        private void UpdateBranchChilds()
        {
            UpdateQuantityBranches();
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
                childs.Add(newBranch);
                k++;
            }

            int currentChilds = childs.Count;
            if (currentChilds > stems_max)
                for (int i = currentChilds; i != stems_max; i--)
                {
                    var childToRemove = childs[childs.Count - 1];
                    childs.RemoveAt(childs.Count - 1);
                    branchManager.RemoveBranch(childToRemove, levelBranch + 1);
                }
            UpdatePropertiesChilds();
        }

        private void UpdatePropertiesChilds()
        {
            var sections = GetDesiredSections();
            for (int i = 0, n = 0; i < sections; i++)
            {
                if (branchData.ShapeCurve == ShapeCurve.Default)
                    UpdateBranchSection(n, i, branchData.NBranchesPerSection, in mainSpline);
                else
                    UpdateBranchSection(n, i, branchData.NBranchesPerSection, in secondarySpline);

                n += branchData.NBranchesPerSection;
            }
        }

        private void UpdateBranchSection(int indexBranch, int indexSection, int branchesPerSection,
            in DynamicSpline spline)
        {
            var sections = GetDesiredSections();
            var stepPositionBranches = sections == 0 ? 0 : 1f / (float)(sections + 1);
            var currStep = (indexSection + 1) * stepPositionBranches;
            var currUp = GetBitangentOnSurface(currStep);

            for (int j = 0; j < branchesPerSection; j++)
            {
                childs[indexBranch].UpdateNormalizePosition(currStep);
                childs[indexBranch].UpdateIndexData(indexBranch + 1, j + 1);
                childs[indexBranch++].Update(scaleUpdated, curveUpdated, numberChildsUpdated, orientationUpdated, updateMesh);
            }
        }

        private int GetDesiredBranches()
        {
            int stems_max = 0;
            int residuo = 0;
            stems_max = Mathf.Max(0, branchData.NBranches);
            int branchesPerSection = Mathf.Max(1, branchData.NBranchesPerSection);
            residuo = stems_max % branchesPerSection;
            stems_max -= residuo;
            return Mathf.Max(0, stems_max);
        }

        private int GetDesiredSections()
        {
            int branchesPerSection = Mathf.Max(1, branchData.NBranchesPerSection);
            return Mathf.Max(0, Mathf.RoundToInt(GetDesiredBranches() / branchesPerSection));
        }

        private void UpdateChildsWithoutParentChange()
        {
            for (int i = 0; i < childs.Count; i++)
                childs[i].Update(false, false, false, orientationUpdated, false);
        }

        public void RemoveChildsInUse()
        {
            if (childs.Count <= 0) return;

            int currentChilds = childs.Count;
            for (int i = currentChilds; i > 0; i--)
            {
                var childToRemove = childs[childs.Count - 1];
                childs.RemoveAt(childs.Count - 1);
                branchManager.RemoveBranch(childToRemove, levelBranch + 1);
            }
        }
    }
}