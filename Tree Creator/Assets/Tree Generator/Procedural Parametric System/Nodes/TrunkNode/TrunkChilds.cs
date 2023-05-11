using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

namespace TreeCreator
{
    public partial class TrunkNode
    {
        private List<IBranchWrite> childs;
        private BranchNodeManager branchManager;
        private bool scaleUpdated, curveUpdated, numberChildsUpdated;


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
                    branchManager.RemoveBranch(childToRemove, 1);
                }
            UpdatePropertiesChilds();
        }

        private void UpdatePropertiesChilds()
        {
            var sections = GetDesiredSections();
            var n = 0;
            int i = 0;
            if (trunkData.NBaseSplits > 0 && trunkData.NBaseSplits <= trunkData.NBranches)
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
                    UpdateBranchSection(n, i, GetBranchesPerSection(), in mainSpline);
                else
                    UpdateBranchSection(n, i, GetBranchesPerSection(), in secondarySpline);

                n += GetBranchesPerSection();
            }
        }

        private void UpdateBranchSection(int indexBranch, int indexSection, int branchesPerSection,
            in DynamicSpline spline)
        {
            var sections = GetDesiredSections();
            var stepPositionBranches = sections == 1 ? 0f : (1f - treeData.BaseSize) / (float)(sections);
            var currStep = (indexSection * stepPositionBranches) + treeData.BaseSize;
            for (int j = 0; j < branchesPerSection; j++)
            {
                childs[indexBranch].UpdateNormalizePosition(currStep);
                childs[indexBranch].UpdateIndexData(indexBranch + 1, j + 1);
                childs[indexBranch++].Update(scaleUpdated, curveUpdated, numberChildsUpdated, false, updateMesh);
            }
        }

        private int GetDesiredBranches()
        {
            int stems_max = 0;
            int residuo = 0;
            int branchesPerSection = GetBranchesPerSection();
            if (trunkData.NBaseSplits > 1 && trunkData.NBaseSplits < trunkData.NBranches)
            {
                stems_max = trunkData.NBranches - trunkData.NBaseSplits;
                residuo = stems_max % branchesPerSection;
                stems_max -= residuo;
                return stems_max + trunkData.NBaseSplits;
            }
            else if (trunkData.NBaseSplits == trunkData.NBranches)
                return trunkData.NBaseSplits;

            stems_max = trunkData.NBranches;
            residuo = stems_max % branchesPerSection;
            stems_max -= residuo;
            return Mathf.Max(0, stems_max);
        }

        private int GetDesiredSections()
        {
            int stems_max = 0;
            int branchesPerSection = 0;
            branchesPerSection = GetBranchesPerSection();
            if (trunkData.NBaseSplits > 1 && trunkData.NBaseSplits <= trunkData.NBranches)
            {
                stems_max = GetDesiredBranches() - trunkData.NBaseSplits;
                stems_max = Mathf.RoundToInt(stems_max / branchesPerSection);
                return stems_max + 1;
            }
            return Mathf.Max(0, Mathf.RoundToInt(GetDesiredBranches() / branchesPerSection));
        }

        private void UpdateChildsWithoutParentChange()
        {
            for (int i = 0; i < childs.Count; i++)
                childs[i].Update(false, false, false, false, updateMesh);
        }

        private void RemoveChildsInUse()
        {
            if (childs.Count <= 0) return;

            int currentChilds = childs.Count;
            for (int i = currentChilds; i > 0; i--)
            {
                var childToRemove = childs[childs.Count - 1];
                childs.RemoveAt(childs.Count - 1);
                branchManager.RemoveBranch(childToRemove, 1);
            }
        }

    }
}