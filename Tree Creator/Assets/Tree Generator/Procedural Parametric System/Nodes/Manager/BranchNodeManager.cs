using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    public class BranchNodeManager : MonoBehaviour
    {
        public GameObject trunkObject;
        public Material branchMaterial;
        private TreeData treeData;

        private List<IBranchWrite>[] levelsBranch = new List<IBranchWrite>[] {
            new List<IBranchWrite>(), new List<IBranchWrite>(), new List<IBranchWrite>(), new List<IBranchWrite>(), new List<IBranchWrite>(),
            new List<IBranchWrite>(), new List<IBranchWrite>(), new List<IBranchWrite>(), new List<IBranchWrite>()
        };

        private int levels = 0;
        private List<IBranchWrite> inactiveBranches = new List<IBranchWrite>();

        public void SetTreeData(TreeData treeData) => this.treeData = treeData;


        public IBranchWrite GetBranch(int levelBranch, IBranchRead parent)
        {
            if (levelBranch <= 0 || levelBranch >= levelsBranch.Length)
                return null;

            UpdateQuantityLevels();

            IBranchWrite newBranch;
            if (inactiveBranches.Count <= 0)
            {
                newBranch = new BranchNode(treeData, levelBranch, parent, this);
                levelsBranch[levelBranch - 1].Add(newBranch);
                return newBranch;
            }
            newBranch = inactiveBranches[inactiveBranches.Count - 1];
            newBranch.UpdateCoreData(levelBranch, parent);
            newBranch.SetActiveObject(trunkObject.transform, true);
            inactiveBranches.RemoveAt(inactiveBranches.Count - 1);
            levelsBranch[levelBranch - 1].Add(newBranch);
            return newBranch;
        }

        private void UpdateQuantityLevels()
        {
            levels = treeData.Levels;
            for (int i = levelsBranch.Length - 1; i >= 0; i--)
            {
                var currentLevelListBranch = levelsBranch[i];
                var currLevelBranch = i + 1;
                if (currLevelBranch > levels && currentLevelListBranch.Count > 0)
                {
                    for (int j = 0; j < currentLevelListBranch.Count; j++)
                    {
                        var currentBranch = currentLevelListBranch[j];
                        currentBranch.SetActiveObject(transform, false);
                        inactiveBranches.Add(currentBranch);
                    }
                    currentLevelListBranch.Clear();
                }
            }
        }

        public bool RemoveBranch(IBranchWrite branch, int levelBranch)
        {
            if (branch == null)
                return false;

            if (levelsBranch[levelBranch - 1].Contains(branch))
            {
                branch.RemoveChildsInUse();
                inactiveBranches.Add(branch);
                branch.SetActiveObject(transform, false);
                return levelsBranch[levelBranch - 1].Remove(branch);
            }
            return false;
        }

        public void UpdateBranches(int level)
        {
            if (level <= 0 || level >= levelsBranch.Length)
                return;

            levels = treeData.Levels;
            int minLevel = level - 1;
            for (int i = 0; i < levelsBranch[minLevel].Count; i++)
            {
                var currBranch = levelsBranch[minLevel][i];
                currBranch.Update(false, false, false, false, false);
            }
        }

        public GameObject GetMeshContainer()
        {
            var branchObject = new GameObject("Mesh");
            branchObject.AddComponent<MeshRenderer>().material = branchMaterial;
            return branchObject;
        }

        public GameObject GetBranchContainer()
        {
            var branchObject = new GameObject("Branch");
            branchObject.transform.SetParent(trunkObject.transform);
            branchObject.transform.localScale = Vector3.one;
            return branchObject;
        }
    }
}