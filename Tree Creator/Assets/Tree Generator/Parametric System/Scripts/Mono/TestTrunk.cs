using System.Collections.Generic;
using UnityEngine;
using TreeCreator;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestTrunk : MonoBehaviour
{
    bool onStart;
    public float sizeCube = 0.2f;
    public int resolutionSplineTrunk;

    public TreeData treeData;
    public BranchManager branchManager;

    public Trunk trunk;
    // private List<Mesh> meshesTree;


    private void OnValidate()
    {
        if (onStart == false) return;
        // trunk.Update(treeData.HasUpdated());
        int levelOfUpdate = treeData.UpdateAtLevel();
        if (levelOfUpdate <= 1)
        {
            bool treeUpdate = treeData.EqualsTreeScale() == false;
            trunk.Update(treeUpdate);
            // trunk.Update(treeData.HasUpdated());
            // trunk.Update(true);
        }
        else
            branchManager.UpdateBranches(levelOfUpdate);

        treeData.UpdateCaching();
        // trunk.Update(levelOfUpdate == 0 ? true : false);
    }

    void Start()
    {
        branchManager.SetTreeData(treeData);
        // meshesTree = new List<Mesh>();
        trunk = new Trunk(treeData, branchManager);
        trunk.Update(true);
        // meshesTree.Add(trunk.GetTrunkMesh());
        // var branches = trunk.GetChilds();
        // RecursionTree(branches, 1);

        // var mesh = new Mesh();
        // mesh.name = "Tree";
        var mesh = trunk.GetTrunkMesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // branchManager.UpdateBranches(1);
        onStart = true;

        // mesh = meshesTree[0];
        // for (int i = 1; i < meshesTree.Count; i++)
        // {
        //     var container = new GameObject("Branch");
        //     container.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
        //     container.AddComponent<MeshFilter>().mesh = meshesTree[i];
        //     container.transform.SetParent(transform);
        // }

        // CombineInstance[] combineInstances = new CombineInstance[meshesTree.Count];
        // for (int i = 0; i < meshesTree.Count; i++)
        // {
        //     combineInstances[i] = new CombineInstance();
        //     combineInstances[i].mesh = meshesTree[i];
        //     combineInstances[i].subMeshIndex = i;
        // }
        // mesh.CombineMeshes(combineInstances, false, false, false);

    }

    // private void OnDisable() {
    //     System.GC.Collect();
    //     System.GC.WaitForPendingFinalizers();
    // }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         var points = trunk.GetPoints();
    //         var lenght = 0f;
    //         for (int i = 0; i < points.Length - 1; i++)
    //             lenght += (points[i] - points[i + 1]).magnitude;
    //         print(lenght);
    //     }
    // }

    // private void OnDrawGizmos()
    // {
    //     if (onStart == false) return;

    //     Gizmos.color = Color.green;
    //     trunk.GetSplineinUse(out MeshGenerator.DynamicSpline spline);
    //     float step = 1f / resolutionSplineTrunk;
    //     for (int i = 0; i <= resolutionSplineTrunk; i++)
    //         Gizmos.DrawCube(spline.GetPoint(i * step), Vector3.one * sizeCube);

    //     var childs = trunk.GetChilds();
    //     Gizmos.color = Color.red;
    //     RecursionTree(childs, 1);


    //     // for (int i = 0; i < trunkPoints.Length; i++)
    //     //     Gizmos.DrawCube(trunkPoints[i], Vector3.one * sizeCube);

    //     // for (int i = 0; i < childs.Length; i++)
    //     // {
    //     //     var currentChild = childs[i].GetPoints();
    //     //     for (int j = 0; j < currentChild.Length; j++)
    //     //         Gizmos.DrawCube(currentChild[j], Vector3.one * sizeCube);
    //     // }
    //     // Gizmos.color = Color.blue;
    //     // var granChilds = childs[0].GetChilds();
    //     // for (int i = 0; i < granChilds.Length; i++)
    //     // {
    //     //     var currentChild = granChilds[i].GetPoints();
    //     //     for (int j = 0; j < currentChild.Length; j++)
    //     //         Gizmos.DrawCube(currentChild[j], Vector3.one * sizeCube);
    //     // }

    //     // trunk.GetAxis(out Vector3 forward, out Vector3 up, out Vector3 right);
    //     // Gizmos.color = Color.blue;
    //     // Gizmos.DrawRay(transform.position, forward * 2f);
    //     // Gizmos.color = Color.red;
    //     // Gizmos.DrawRay(transform.position, right * 2f);
    //     // Gizmos.color = Color.green;
    //     // Gizmos.DrawRay(transform.position, up * 2f);
    // }

    // private void RecursionTree(TreeCreator.Branch[] currentBranches, int level = 1)
    // {
    //     if (currentBranches == null)
    //         return;
    //     if (level > treeData.levels)
    //         return;

    //     float sizeOffset = GetSize(level);

    //     for (int i = 0; i < currentBranches.Length; i++)
    //     {
    //         var currentPoints = currentBranches[i].GetPoints();
    //         for (int j = 0; j < currentPoints.Length; j++)
    //             Gizmos.DrawCube(currentPoints[j], Vector3.one * sizeCube * sizeOffset);
    //     }
    //     for (int i = 0; i < currentBranches.Length; i++)
    //     {
    //         var nextChilds = currentBranches[i].GetChilds();
    //         RecursionTree(nextChilds, level + 1);
    //     }
    // }

    // private void RecursionTree(TreeCreator.Branch[] currentBranches, int level = 1)
    // {
    //     if (currentBranches == null)
    //         return;
    //     if (level > treeData.levels)
    //         return;

    //     for (int i = 0; i < currentBranches.Length; i++)
    //     {
    //         meshesTree.Add(currentBranches[i].GetBranchMesh());
    //         // var currentPoints = currentBranches[i].GetPoints();
    //         // for (int j = 0; j < currentPoints.Length; j++)
    //         //     Gizmos.DrawCube(currentPoints[j], Vector3.one * sizeCube * sizeOffset);
    //     }
    //     for (int i = 0; i < currentBranches.Length; i++)
    //     {
    //         var nextChilds = currentBranches[i].GetChilds();
    //         RecursionTree(nextChilds, level + 1);
    //     }
    // }

    // private float GetSize(int level)
    // {
    //     switch (level)
    //     {
    //         case 1:
    //             return 0.75f;
    //         case 2:
    //             return 0.35f;
    //         case 3:
    //             return 0.15f;
    //         default:
    //             return 0.05f;
    //     }
    // }

}
