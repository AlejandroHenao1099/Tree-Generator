using System.Collections.Generic;
using UnityEngine;

namespace LindenmayerSystems
{
    public class TreeNode
    {
        private TreeNode parent;
        private List<TreeNode> brothers;
        // private TreeNode child;
        private Vector3 position;
        private float radius;

        public Vector3 Position { get => position; }
        // public Vector3 Direction { get => (position - parent.Position).normalized; }
        public float Radius { get => radius; }
        public TreeNode[] Brothers { get => brothers.ToArray(); }
        public int BrotherCount { get => brothers.Count; }
        public bool hasBrothers { get => brothers.Count != 0; }
        public TreeNode Child { get; set; }

        public TreeNode(TreeNode parent, Vector3 position, float radius)
        {
            this.parent = parent;
            this.position = position;
            this.radius = radius;
            brothers = new List<TreeNode>();
        }

        public TreeNode GetParent() => parent;

        public void AddBrother(TreeNode brother)
        {
            brothers.Add(brother);
        }

        public TreeNode GetBrother(int index)
        {
            index = Mathf.Clamp(index, 0, BrotherCount - 1);
            return brothers[index];
        }

        public TreeNode GetLastBrother()
        {
            return brothers[BrotherCount - 1];
        }
    }
}