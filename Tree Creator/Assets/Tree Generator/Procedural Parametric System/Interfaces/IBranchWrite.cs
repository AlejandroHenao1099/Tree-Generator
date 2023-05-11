using UnityEngine;

namespace TreeCreator
{
    public interface IBranchWrite
    {
        public void Update(bool parentScaleUpdated, bool parentCurveUpdated,
            bool parentNumberChildsUpdated, bool parentUpdatedOrientation, bool parentMeshUpdated);
            
        public void UpdateNormalizePosition(float newPos);
        public void UpdateIndexData(int indexBranch, int localIndex);
        public void UpdateCoreData(int levelBranch, IBranchRead parent);
        public void RemoveChildsInUse();
        public void SetActiveObject(Transform parent, bool active);
    }
}