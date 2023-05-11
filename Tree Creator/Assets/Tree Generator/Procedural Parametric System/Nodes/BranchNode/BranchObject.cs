using UnityEngine;

namespace TreeCreator
{
    public partial class BranchNode
    {
        private GameObject branchContainer;
        private GameObject branchMesh;
        private Transform transformContainer;
        private Transform transformMesh;

        private void InitializeObject()
        {
            branchContainer = branchManager.GetBranchContainer();
            branchMesh = branchManager.GetMeshContainer();
            branchMesh.AddComponent<MeshFilter>().mesh = mesh;
            transformContainer = branchContainer.transform;
            transformMesh = branchMesh.transform;
            branchMesh.transform.SetParent(transformContainer);
            branchMesh.transform.localScale = Vector3.one;
        }

        public void UpdateOrientation()
        {
            float turnAngle = GetTurnAngle();
            parent.GetPNBOnSurface(normalizedPosition, turnAngle, 
                out Vector3 currPos, out Vector3 newForward, out Vector3 newUp);
            
            transformContainer.localPosition = currPos;
            transformContainer.localRotation = Quaternion.LookRotation(newForward, newUp);
            transformMesh.localRotation = Quaternion.Euler(GetPitchAngle(), 0, 0);
        }

        public void SetActiveObject(Transform parent, bool active)
        {
            transformContainer.SetParent(parent);
            transformContainer.localPosition = Vector3.zero;
            transformContainer.localScale = Vector3.one;
            branchContainer.SetActive(active);
        }

    }
}