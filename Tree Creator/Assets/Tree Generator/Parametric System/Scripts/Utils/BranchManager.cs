using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    public class BranchManager : MonoBehaviour
    {
        public Material materialBranch;
        public GameObject trunkObject;
        private TreeData treeData;

        // private List<Branch> activeBranches = new List<Branch>();
        // private List<GameObject> activeBranchesObject = new List<GameObject>();

        private List<Branch>[] levelsBranch = new List<Branch>[] {
            new List<Branch>(), new List<Branch>(), new List<Branch>(), new List<Branch>(), new List<Branch>(),
            new List<Branch>(), new List<Branch>(), new List<Branch>(), new List<Branch>()
        };

        private List<GameObject>[] levelsBranchObject = new List<GameObject>[] {
            new List<GameObject>(), new List<GameObject>(), new List<GameObject>(),
            new List<GameObject>(), new List<GameObject>(), new List<GameObject>(),
            new List<GameObject>(), new List<GameObject>(), new List<GameObject>()
        };

        private int levels = 0;

        private List<Branch> inactiveBranches = new List<Branch>();
        private List<GameObject> inactiveBranchesObject = new List<GameObject>();

        // public int QuantityBranches { get => activeBranches.Count; }
        public void SetTreeData(TreeData treeData) => this.treeData = treeData;


        public Branch GetBranch(int levelBranch, IBranch parent)
        {
            if (levelBranch <= 0 || levelBranch >= levelsBranch.Length)
                return null;

            UpdateQuantityLevels();

            Branch newBranch;
            if (inactiveBranches.Count <= 0)
            {
                newBranch = new Branch(treeData, levelBranch, parent, this);
                AddBranchToLevel(newBranch, levelBranch, true);
                // activeBranches.Add(newBranch);
                // CreateBranchObject(newBranch);
                return newBranch;
            }
            newBranch = inactiveBranches[inactiveBranches.Count - 1];
            newBranch.UpdateCoreData(levelBranch, parent);
            inactiveBranches.RemoveAt(inactiveBranches.Count - 1);
            // activeBranches.Add(newBranch);
            AddBranchToLevel(newBranch, levelBranch);

            // ActiveBranch();
            return newBranch;
        }

        private void UpdateQuantityLevels()
        {
            levels = treeData.Levels;
            for (int i = levelsBranch.Length - 1; i >= 0; i--)
            {
                var currentLevelListBranch = levelsBranch[i];
                var currentLevelListObject = levelsBranchObject[i];
                var currLevelBranch = i + 1;
                if (currLevelBranch > levels && currentLevelListBranch.Count > 0)
                {
                    for (int j = 0; j < currentLevelListBranch.Count; j++)
                    {
                        var currentBranch = currentLevelListBranch[j];
                        var currentBranchObj = currentLevelListObject[j];
                        currentBranchObj.transform.SetParent(transform);
                        currentBranchObj.transform.localScale = Vector3.one;
                        currentBranchObj.SetActive(false);

                        inactiveBranches.Add(currentBranch);
                        inactiveBranchesObject.Add(currentBranchObj);
                    }
                    currentLevelListBranch.Clear();
                    currentLevelListObject.Clear();
                }
            }
        }

        private void AddBranchToLevel(Branch branch, int level, bool createNewObject = false)
        {
            GameObject branchObject;
            if (createNewObject == true)
                branchObject = CreateBranchObject(branch);
            else
            {
                branchObject = inactiveBranchesObject[inactiveBranchesObject.Count - 1];
                inactiveBranchesObject.RemoveAt(inactiveBranchesObject.Count - 1);
            }

            branchObject.transform.SetParent(trunkObject.transform);
            branchObject.transform.localScale = Vector3.one;
            branchObject.transform.localRotation = Quaternion.identity;
            branchObject.SetActive(true);

            levelsBranch[level - 1].Add(branch);
            levelsBranchObject[level - 1].Add(branchObject);

            // activeBranches.Add(newBranch);
            // CreateBranchObject(newBranch);
        }

        private GameObject CreateBranchObject(Branch branch)
        {
            var branchObject = new GameObject("Branch");
            branchObject.AddComponent<MeshRenderer>().material = materialBranch;
            branchObject.AddComponent<MeshFilter>().mesh = branch.GetBranchMesh();
            // branchObject.transform.SetParent(trunkObject.transform);
            // branchObject.transform.localScale = Vector3.one;
            return branchObject;
            // activeBranchesObject.Add(branchObject);
        }

        private void ActiveBranch()
        {
            var branchObject = inactiveBranchesObject[inactiveBranchesObject.Count - 1];
            inactiveBranchesObject.RemoveAt(inactiveBranchesObject.Count - 1);
            branchObject.transform.SetParent(trunkObject.transform);
            branchObject.transform.localScale = Vector3.one;
            branchObject.SetActive(true);
            // activeBranchesObject.Add(branchObject);
        }

        public bool RemoveBranch(Branch branch)
        {
            if (branch == null)
                return false;

            int levelBranch = branch.GetLevelBranch();
            // if ( activeBranches.Contains(branch))
            if (levelsBranch[levelBranch - 1].Contains(branch))
            {
                branch.RemoveChildsInUse();
                inactiveBranches.Add(branch);
                DisableBranchAtLevel(branch, levelBranch);
                return levelsBranch[levelBranch - 1].Remove(branch);
            }
            return false;
        }

        private void DisableBranchAtLevel(Branch branch, int level)
        {
            int indexBranch = levelsBranch[level - 1].IndexOf(branch);
            var branchObject = levelsBranchObject[level - 1][indexBranch];
            branchObject.transform.SetParent(transform);
            branchObject.transform.localScale = Vector3.one;
            branchObject.transform.localRotation = Quaternion.identity;
            branchObject.SetActive(false);

            levelsBranchObject[level - 1].RemoveAt(indexBranch);
            inactiveBranchesObject.Add(branchObject);
        }

        public void UpdateBranches(int level)
        {
            if (level < 0 || level >= levelsBranchObject.Length)
                return;

            levels = treeData.Levels;
            int minLevel = level == 0 ? 0 : level - 2;
            for (int i = 0; i < levelsBranch[minLevel].Count; i++)
            {
                var currBranch = levelsBranch[minLevel][i];
                currBranch.Update(false);
            }
            // for (int n = minLevel; n < levels; n++)
            // {
            //     for (int i = 0; i < levelsBranchObject[n].Count; i++)
            //     {
            //         var currBranch = levelsBranch[n][i];
            //         var currTransform = levelsBranchObject[n][i].transform;
            //         var currentOrientation = currBranch.GetOrientationBranch();
            //         var currentPosition = currentOrientation.GetPosition();
            //         var currentRotation = Quaternion.LookRotation(
            //                 currentOrientation.GetForward(), currentOrientation.GetUp());
            //         currTransform.localPosition = currentPosition;
            //         currTransform.localRotation = currentRotation;
            //         currTransform.localScale = Vector3.one;
            //     }
            // }
        }
    }
}