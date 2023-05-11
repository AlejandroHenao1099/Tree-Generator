using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEngine.Mathf;

namespace LindenmayerSystems
{

    public class TurtleParametric2D
    {
        private string axiom;
        private TurtleParametricRules turtle;

        private List<Vector2Int> points;
        Stack<Vector2Int> stackPositions;
        Stack<float> stackAngles;

        Vector2 heading;

        Vector2Int lower;
        Vector2Int greater;

        public TurtleParametric2D(string axiom, TurtleParametricRules turtle)
        {
            this.axiom = axiom;
            this.turtle = turtle;
            points = new List<Vector2Int>();
            stackPositions = new Stack<Vector2Int>();
            stackAngles = new Stack<float>();
            lower = new Vector2Int(int.MaxValue, int.MaxValue);
            greater = new Vector2Int(int.MinValue, int.MinValue);
        }

        public byte[,] Create()
        {
            InitializeVariables();
            EvaluateAxiom();
            var resolution = NormalizePoints();
            byte[,] matrix = new byte[resolution, resolution];
            LineOnMatrix.DrawLine(matrix, points);
            return matrix;
        }

        private void InitializeVariables()
        {
            stackPositions.Push(Vector2Int.zero);
            stackAngles.Push(turtle.initialRotation);

            heading = RotateHeading(stackAngles.Peek());
            points.Add(stackPositions.Peek());
            UpdateCorners(stackPositions.Peek());
        }

        private void EvaluateAxiom()
        {
            int turtleOrientation = turtle.invertRotation == false ? 1 : -1;
            for (int i = 0, len = axiom.Length; i < len; i++)
            {
                switch (axiom[i])
                {
                    case 'F':
                        MoveAndDraw(ref i);
                        break;
                    case 'f':
                        var currentVoid = stackPositions.Pop();
                        stackPositions.Push(CalculateNextPosition(currentVoid, heading, turtle.LenghtMovement));
                        points.Add(new Vector2Int(int.MinValue, int.MinValue));
                        break;
                    case '+':
                        Rotate(ref i, -1, turtleOrientation);
                        break;
                    case '-':
                        Rotate(ref i, 1, turtleOrientation);
                        break;
                    case '[':
                        stackPositions.Push(stackPositions.Peek());
                        stackAngles.Push(stackAngles.Peek());
                        break;
                    case ']':
                        stackPositions.Pop();
                        stackAngles.Pop();
                        heading = RotateHeading(stackAngles.Peek());
                        points.Add(new Vector2Int(int.MinValue, int.MinValue));
                        points.Add(stackPositions.Peek());
                        break;
                }
            }
        }

        private void MoveAndDraw(ref int currentIndex)
        {
            var currentPosition = stackPositions.Pop();
            if (SearchFunction(currentIndex))
            {
                var endIndex = SearchEndFunction(currentIndex);
                var lengthMovement = GetValueFunction(currentIndex + 1, endIndex);
                stackPositions.Push(CalculateNextPosition(currentPosition, heading, lengthMovement * turtle.LenghtMovement));
                currentIndex += (endIndex - currentIndex);
            }
            else
                stackPositions.Push(CalculateNextPosition(currentPosition, heading, turtle.LenghtMovement));
            points.Add(stackPositions.Peek());
            UpdateCorners(stackPositions.Peek());
        }

        private void Rotate(ref int currentIndex, float orientation, float turtleOrientation)
        {
            var currentAngle = 0f;
            if (SearchFunction(currentIndex))
            {
                var endIndex = SearchEndFunction(currentIndex);
                currentAngle = stackAngles.Pop() +
                    (GetValueFunction(currentIndex + 1, endIndex) * orientation) * turtleOrientation;
                currentIndex += (endIndex - currentIndex);
            }
            else
                currentAngle = stackAngles.Pop() + (turtle.AngleTurn * orientation) * turtleOrientation;

            stackAngles.Push(currentAngle);
            heading = RotateHeading(stackAngles.Peek());
        }

        private Vector2 RotateHeading(float angles)
        {
            return new Vector2(Cos(angles * Deg2Rad),
                    Sin(angles * Deg2Rad));
        }

        private Vector2Int CalculateNextPosition(Vector2Int position, Vector2 heading, float length)
        {
            Vector2 newPosition = (Vector2)position + heading * length;
            return new Vector2Int(RoundToInt(newPosition.x), RoundToInt(newPosition.y));
        }

        private Vector2Int CalculateNextPosition(Vector2Int position, Vector2 heading, int length, float tropism)
        {
            Vector2 newPosition = (Vector2)position + heading * length;
            return new Vector2Int(RoundToInt(newPosition.x), RoundToInt(newPosition.y));
        }

        private void UpdateCorners(Vector2Int position)
        {
            lower = new Vector2Int(Min(position.x, lower.x), Min(position.y, lower.y));
            greater = new Vector2Int(Max(position.x, greater.x), Max(position.y, greater.y));
        }

        private int NormalizePoints()
        {
            var resolution = Abs(lower.x - greater.x) > Abs(lower.y - greater.y)
                    ? Abs(lower.x - greater.x) : Abs(lower.y - greater.y);
            resolution++;

            for (int i = 0, len = points.Count; i < len; i++)
            {
                var multi = points[i].x == int.MinValue ? 0 : 1;
                points[i] -= (lower * multi);
            }
            return resolution;
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