using System.Collections.Generic;
using UnityEngine;

namespace LindenmayerSystems
{
    public class TurtleParametric3D
    {
        Stack<Matrix4x4> stackMatrix;
        private List<SplineData> procesedSplinesData;
        private Stack<SplineData> stackSplineData;
        private Stack<TreeNode> treeNodes;

        private List<Splines> splines;

        private string axiom;
        private TurtleRules3D turtle;
        private float currentRadius = 1;
        private bool first = true;
        private bool branching = false;

        public TurtleParametric3D(string axiom, TurtleRules3D turtle)
        {
            this.axiom = axiom;
            this.turtle = turtle;
            stackMatrix = new Stack<Matrix4x4>();
            stackSplineData = new Stack<SplineData>();
            procesedSplinesData = new List<SplineData>();
            treeNodes = new Stack<TreeNode>();
            splines = new List<Splines>();
        }

        public List<SplineData> Create()
        {
            InitializeData();
            return procesedSplinesData;
        }

        private void InitializeData()
        {
            var initialMatrix = new Matrix4x4();
            initialMatrix.SetTRS(Vector3.zero, Quaternion.Euler(-90, 0, 0), Vector3.one);
            stackMatrix.Push(initialMatrix);

            var currentPosition = GetPosition(initialMatrix);
            stackSplineData.Push(new SplineData(0));
            // stackSplineData.Peek().AddPoint(currentPosition);
            // stackSplineData.Peek().AddWidth(turtle.initialWidth);
            EvaluateAxiom();
        }

        private void EvaluateAxiom()
        {
            int mulRot = turtle.invertRotation == false ? 1 : -1;

            for (int i = 0, len = axiom.Length; i < len; i++)
            {
                switch (axiom[i])
                {
                    case 'F':
                        UpdatePositionAndDraw(ref i);
                        break;
                    case 'f':
                        UpdatePositionAndNoDraw(ref i);
                        break;
                    case '+':
                        RotateTurtle(ref i, RotationType.Turn, 1, mulRot);
                        break;
                    case '-':
                        RotateTurtle(ref i, RotationType.Turn, -1, mulRot);
                        break;
                    case '&':
                        RotateTurtle(ref i, RotationType.Pitch, 1, mulRot);
                        break;
                    case '^':
                        RotateTurtle(ref i, RotationType.Pitch, -1, mulRot);
                        break;
                    case '\\':
                        RotateTurtle(ref i, RotationType.Roll, 1, mulRot);
                        break;
                    case '/':
                        RotateTurtle(ref i, RotationType.Roll, -1, mulRot);
                        break;
                    case '|':
                        Turn180();
                        break;
                    case '!':
                        SetRadius(ref i);
                        break;
                    case '$':
                        AlignHorizontal();
                        break;
                    case '[':
                        StartBranch();
                        break;
                    case ']':
                        FinishBranch();
                        break;
                }
            }
            FillSplineData();
        }

        private void UpdatePositionAndDraw(ref int currentIndex)
        {
            if (first)
            {
                var matrix = stackMatrix.Peek();
                var position = GetPosition(matrix);
                var direction = GetForward(matrix);
                treeNodes.Push(new TreeNode(null, position, currentRadius));

                stackSplineData.Peek().AddPoint(GetPosition(stackMatrix.Peek()));
                stackSplineData.Peek().AddWidth(currentRadius);
                first = false;
            }
            var endIndex = SearchEndFunction(currentIndex);
            var lengthMovement = GetValueFunction(currentIndex + 1, endIndex);
            UpdatePosition(lengthMovement);
            currentIndex += (endIndex - currentIndex);
            stackSplineData.Peek().AddPoint(GetPosition(stackMatrix.Peek()));
            stackSplineData.Peek().AddWidth(currentRadius);

            UpdateNode();
        }

        private void UpdatePositionAndNoDraw(ref int currentIndex)
        {
            var endIndex = SearchEndFunction(currentIndex);
            var lengthMovement = GetValueFunction(currentIndex + 1, endIndex);
            UpdatePosition(lengthMovement);
            currentIndex += (endIndex - currentIndex);
        }

        private void UpdatePosition(float lengthDisplacement)
        {
            var matrix = stackMatrix.Pop();

            var currentPosition = GetPosition(matrix);
            var forward = matrix.MultiplyVector(Vector3.forward);
            currentPosition += (forward * (turtle.LenghtMovement * lengthDisplacement));
            matrix.SetTRS(currentPosition, matrix.rotation, Vector3.one);

            matrix = CalculateTropism(matrix);
            stackMatrix.Push(matrix);
        }

        private void RotateTurtle(ref int currentIndex, RotationType rotationType, float orientation, float turtleOrientation)
        {
            var endIndex = SearchEndFunction(currentIndex);
            var angles = GetValueFunction(currentIndex + 1, endIndex) * turtleOrientation * orientation;

            var matrix = stackMatrix.Pop();
            var currentRotation = matrix.rotation;
            var currentPosition = GetPosition(matrix);
            switch (rotationType)
            {
                case RotationType.Turn:
                    currentRotation *= Quaternion.Euler(0, angles, 0);
                    matrix.SetTRS(currentPosition, currentRotation, Vector3.one);
                    break;
                case RotationType.Pitch:
                    currentRotation *= Quaternion.Euler(angles, 0, 0);
                    matrix.SetTRS(currentPosition, currentRotation, Vector3.one);
                    break;
                case RotationType.Roll:
                    currentRotation *= Quaternion.Euler(0, 0, angles);
                    matrix.SetTRS(currentPosition, currentRotation, Vector3.one);
                    break;
            }
            // matrix = CalculateTropism(matrix);
            stackMatrix.Push(matrix);
            currentIndex += (endIndex - currentIndex);
        }

        private Matrix4x4 CalculateTropism(Matrix4x4 matrix)
        {
            var forward = matrix.MultiplyVector(Vector3.forward).normalized;
            var target = Quaternion.LookRotation(turtle.directionTropism.normalized);
            var newRotation = Quaternion.Slerp(matrix.rotation, target, turtle.tropism);
            matrix.SetTRS(GetPosition(matrix), newRotation, Vector3.one);
            return matrix;
        }

        private void Turn180()
        {
            var matrix = stackMatrix.Pop();
            var newRotation = matrix.rotation * Quaternion.Euler(0, 180, 0);
            matrix.SetTRS(GetPosition(matrix), newRotation, Vector3.one);
            stackMatrix.Push(matrix);
        }

        private void SetRadius(ref int currentIndex)
        {
            var endIndex = SearchEndFunction(currentIndex);
            var radius = GetValueFunction(currentIndex + 1, endIndex);
            currentRadius = radius;
            currentIndex += (endIndex - currentIndex);
        }

        private void AlignHorizontal()
        {
            var matrix = stackMatrix.Pop();
            var currentPosition = GetPosition(matrix);
            var forward = matrix.MultiplyVector(Vector3.forward);
            var cross = Vector3.Cross(Vector3.down, forward);
            var right = cross / cross.magnitude;
            var up = Vector3.Cross(forward, right);

            var newRotation = Quaternion.LookRotation(forward, up);
            matrix.SetTRS(currentPosition, newRotation, Vector3.one);
            stackMatrix.Push(matrix);
        }

        private void StartBranch()
        {
            stackMatrix.Push(stackMatrix.Peek());
            stackSplineData.Push(new SplineData(0));
            stackSplineData.Peek().AddPoint(GetPosition(stackMatrix.Peek()));
            stackSplineData.Peek().AddWidth(currentRadius);

            branching = true;
        }

        private void FinishBranch()
        {
            stackMatrix.Pop();
            procesedSplinesData.Add(stackSplineData.Pop());
            currentRadius = stackSplineData.Peek().GetLastWidth();

            treeNodes.Pop();
        }

        private void UpdateNode()
        {
            var matrix = stackMatrix.Peek();
            var position = GetPosition(matrix);
            var currentNode = GetCurrentNode();
            if (branching)
            {
                var newNode = new TreeNode(null, position, currentRadius);
                currentNode.AddBrother(newNode);
                treeNodes.Push(newNode);
                branching = false;
            }
            else
                currentNode.Child = new TreeNode(currentNode, position, currentRadius);
        }

        private TreeNode GetCurrentNode()
        {
            var currentNode = treeNodes.Peek();
            while (currentNode.Child != null)
                currentNode = currentNode.Child;
            return currentNode;
        }

        private Vector3 GetPosition(Matrix4x4 matrix)
        {
            return matrix.MultiplyPoint3x4(Vector3.zero);
        }

        private Vector3 GetForward(Matrix4x4 matrix)
        {
            return matrix.MultiplyVector(Vector3.forward);
        }

        public TreeNode GetTree()
        {
            foreach (var tree in treeNodes)
                return tree;
            return null;
        }

        private void FillSplineData()
        {
            for (int i = 0; i < stackSplineData.Count; i++)
                procesedSplinesData.Add(stackSplineData.Pop());
        }

        private bool SearchFunction(int currentIndex)
        {
            if (currentIndex + 1 < axiom.Length && axiom[currentIndex + 1] == '(')
                return true;
            return false;
        }

        private int SearchEndFunction(int currentIndex)
        {
            return axiom.IndexOf(')', currentIndex);
        }

        private float GetValueFunction(int start, int end)
        {
            var number = axiom.Substring(start + 1, end - start - 1);
            return float.Parse(number);
        }
    }
}
