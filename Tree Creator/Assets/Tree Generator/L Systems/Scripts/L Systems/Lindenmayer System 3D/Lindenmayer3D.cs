using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace LindenmayerSystems
{
    public class Lindenmayer3D
    {
        private List<Vector3> points;
        private string axiom;
        private TurtleRules3D turtle;

        private Matrix3x3 rotationMatrix;

        private readonly Vector3 MinPoint = new Vector3(int.MinValue, int.MinValue, int.MinValue);

        public Lindenmayer3D(string axiom, TurtleRules3D turtle)
        {
            this.axiom = axiom;
            this.turtle = turtle;
            // orientation = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
            rotationMatrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
            points = new List<Vector3>();
        }

        public List<Vector3> Create()
        {
            EvaluateAxiom(new List<Vector3>());
            return points;
        }

        public List<Vector3> Create(List<Vector3> leavePoints)
        {
            // leavePoints = new List<Vector3>();
            if(leavePoints == null)
                throw new System.Exception("'leavePoints' must not be null");
            EvaluateAxiom(leavePoints);
            return points;
        }

        private void EvaluateAxiom(List<Vector3> leaves)
        {
            Stack<Vector3> stackPositions = new Stack<Vector3>();
            Stack<Matrix3x3> stackOrientations = new Stack<Matrix3x3>();

            Matrix3x3 orientation = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
            orientation.SetColumn(0, new Vector3(0, 0, 1));
            orientation.SetColumn(1, new Vector3(1, 0, 0));
            orientation.SetColumn(2, new Vector3(0, 1, 0));

            stackPositions.Push(Vector3.zero);
            stackOrientations.Push(orientation);

            Vector3 heading = stackOrientations.Peek().Forward;
            Matrix3x3 currentOrientation;
            points.Add(stackPositions.Peek());

            int orientationRotation = turtle.invertRotation == false ? 1 : -1;
            for (int i = 0, len = axiom.Length; i < len; i++)
            {
                switch (axiom[i])
                {
                    case 'F':
                        var currentPosition = stackPositions.Pop();
                        stackPositions.Push(CalculateNextPosition(currentPosition, stackOrientations.Peek().Forward));
                        points.Add(stackPositions.Peek());
                        break;
                    case 'f':
                        var currentVoid = stackPositions.Pop();
                        stackPositions.Push(CalculateNextPosition(currentVoid, stackOrientations.Peek().Forward));
                        points.Add(MinPoint);
                        break;
                    case '+':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = TurnHeading(currentOrientation, turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '-':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = TurnHeading(currentOrientation, -turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '&':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = PitchHeading(currentOrientation, turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '^':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = PitchHeading(currentOrientation, -turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '\\':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = RollHeading(currentOrientation, turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '/':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = RollHeading(currentOrientation, -turtle.AngleTurn * orientationRotation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '|':
                        currentOrientation = stackOrientations.Pop();
                        currentOrientation = TurnAroundHeading(currentOrientation);
                        stackOrientations.Push(currentOrientation);
                        break;
                    case '[':
                        stackPositions.Push(stackPositions.Peek());
                        stackOrientations.Push(stackOrientations.Peek());
                        break;
                    case ']':
                        stackPositions.Pop();
                        stackOrientations.Pop();
                        points.Add(MinPoint);
                        points.Add(stackPositions.Peek());
                        break;
                    case 'L':
                        leaves.Add(stackPositions.Peek());
                        break;
                        // case '{':
                        //     stackPositions.Pop();
                        //     stackOrientations.Pop();
                        //     points.Add(MinPoint);
                        //     points.Add(stackPositions.Peek());
                        //     break;
                        // case '}':
                        //     stackPositions.Pop();
                        //     stackOrientations.Pop();
                        //     points.Add(MinPoint);
                        //     points.Add(stackPositions.Peek());
                        //     break;
                }
            }
        }

        private Matrix3x3 TurnHeading(Matrix3x3 orientation, float angles)
        {
            rotationMatrix.SetRow(0, new Vector3(Cos(angles * Deg2Rad), Sin(angles * Deg2Rad), 0));
            rotationMatrix.SetRow(1, new Vector3(-Sin(angles * Deg2Rad), Cos(angles * Deg2Rad), 0));
            rotationMatrix.SetRow(2, new Vector3(0, 0, 1));
            return orientation * rotationMatrix;
            // return orientation.Forward;
        }

        private Matrix3x3 PitchHeading(Matrix3x3 orientation, float angles)
        {
            rotationMatrix.SetRow(0, new Vector3(Cos(angles * Deg2Rad), 0, -Sin(angles * Deg2Rad)));
            rotationMatrix.SetRow(1, new Vector3(0, 1, 0));
            rotationMatrix.SetRow(2, new Vector3(Sin(angles * Deg2Rad), 0, Cos(angles * Deg2Rad)));
            return orientation * rotationMatrix;
            // return orientation.Forward;
        }

        private Matrix3x3 RollHeading(Matrix3x3 orientation, float angles)
        {
            rotationMatrix.SetRow(0, new Vector3(1, 0, 0));
            rotationMatrix.SetRow(1, new Vector3(0, Cos(angles * Deg2Rad), -Sin(angles * Deg2Rad)));
            rotationMatrix.SetRow(2, new Vector3(0, Sin(angles * Deg2Rad), Cos(angles * Deg2Rad)));
            return orientation * rotationMatrix;
            // return orientation.Forward;
        }

        private Matrix3x3 TurnAroundHeading(Matrix3x3 orientation)
        {
            return TurnHeading(orientation, 180);
        }

        private Vector3 CalculateNextPosition(Vector3 position, Vector3 heading)
        {
            return position + heading * turtle.LenghtMovement;
        }
    }
}