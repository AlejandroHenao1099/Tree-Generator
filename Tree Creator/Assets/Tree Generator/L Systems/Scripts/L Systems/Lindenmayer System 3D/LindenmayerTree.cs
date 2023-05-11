using System.Collections.Generic;
using UnityEngine;

namespace LindenmayerSystems
{
    public class LindenmayerTree
    {
        Stack<Vector3> stackPositions;
        Stack<Matrix3x3> stackOrientations;
        Stack<float> stackLenghts;
        private List<SplineData> procesedSplinesData;
        private Stack<SplineData> stackSplineData;

        private List<Splines> splines;


        private string axiom;
        private TurtleRules3D turtle;
        private Matrix3x3 rotationMatrix;

        private float currentWidth;
        private float currentLength;

        public LindenmayerTree(string axiom, TurtleRules3D turtle)
        {
            this.axiom = axiom;
            this.turtle = turtle;
            rotationMatrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
            stackPositions = new Stack<Vector3>();
            stackOrientations = new Stack<Matrix3x3>();
            stackLenghts = new Stack<float>();

            stackSplineData = new Stack<SplineData>();
            procesedSplinesData = new List<SplineData>();

            splines = new List<Splines>();
            // branchesOnProcess = new Stack<List<Vector3>>();
            // branchesProcessed = new Stack<List<Vector3>>();
        }

        public List<SplineData> Create()
        {
            InitializeData();
            return procesedSplinesData;
        }

        private void InitializeData()
        {
            stackPositions.Push(Vector3.zero);
            stackOrientations.Push(Matrix3x3.AlignZ);
            stackLenghts.Push(turtle.LenghtMovement);

            currentWidth = turtle.initialWidth;
            stackSplineData.Push(new SplineData(0));
            stackSplineData.Peek().AddPoint(stackPositions.Peek());
            stackSplineData.Peek().AddWidth(currentWidth);
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
                        UpdatePositionAndDraw();
                        break;
                    case 'f':
                        UpdatePositionAndNoDraw();
                        break;
                    case '+':
                        RotateTurtle(RotationType.Turn, turtle.AngleTurn * mulRot);
                        break;
                    case '-':
                        RotateTurtle(RotationType.Turn, -turtle.AngleTurn * mulRot);
                        break;
                    case '&':
                        RotateTurtle(RotationType.Pitch, turtle.AngleTurn * mulRot);
                        break;
                    case '^':
                        RotateTurtle(RotationType.Pitch, -turtle.AngleTurn * mulRot);
                        break;
                    case '\\':
                        RotateTurtle(RotationType.Roll, turtle.AngleTurn * mulRot);
                        break;
                    case '/':
                        RotateTurtle(RotationType.Roll, -turtle.AngleTurn * mulRot);
                        break;
                    case '|':
                        RotateTurtle(RotationType.Turn, 180);
                        break;
                    case '?':
                        currentWidth = currentWidth - 0.3f <= 0 ? 0.1f : currentWidth - 0.3f;
                        break;
                    case '!':
                        var currentLength = stackLenghts.Pop();
                        stackLenghts.Push(currentLength - 0.1f <= 0 ? 0.1f : currentLength - 0.1f);
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

        private void UpdatePositionAndDraw()
        {
            UpdatePosition();
            stackSplineData.Peek().AddPoint(stackPositions.Peek());
            stackSplineData.Peek().AddWidth(currentWidth);
        }

        private void UpdatePositionAndNoDraw()
        {
            UpdatePosition();
            procesedSplinesData.Add(stackSplineData.Pop());
        }

        private void UpdatePosition()
        {
            var newPos = stackPositions.Pop() + stackOrientations.Peek().Forward * stackLenghts.Peek();
            stackPositions.Push(newPos);
        }

        private void RotateTurtle(RotationType rotationType, float angles)
        {
            var currentOrientation = stackOrientations.Pop();
            switch (rotationType)
            {
                case RotationType.Turn:
                    currentOrientation = RotationMatrix.TurnMatrix(currentOrientation, angles);
                    break;
                case RotationType.Pitch:
                    currentOrientation = RotationMatrix.PitchMatrix(currentOrientation, angles);
                    break;
                case RotationType.Roll:
                    currentOrientation = RotationMatrix.RollMatrix(currentOrientation, angles);
                    break;
            }
            stackOrientations.Push(currentOrientation);
        }

        private void StartBranch()
        {
            stackPositions.Push(stackPositions.Peek());
            stackOrientations.Push(stackOrientations.Peek());
            stackLenghts.Push(stackLenghts.Peek());

            stackSplineData.Push(new SplineData(0));

            stackSplineData.Peek().AddPoint(stackPositions.Peek());
            stackSplineData.Peek().AddWidth(currentWidth);
        }

        private void FinishBranch()
        {
            stackPositions.Pop();
            stackOrientations.Pop();
            stackLenghts.Pop();
            procesedSplinesData.Add(stackSplineData.Pop());
            currentWidth = stackSplineData.Peek().GetLastWidth();
        }

        private void FillSplineData()
        {
            for (int i = 0; i < stackSplineData.Count; i++)
                procesedSplinesData.Add(stackSplineData.Pop());
        }
    }

    public enum RotationType
    {
        Turn, Pitch, Roll
    }
}