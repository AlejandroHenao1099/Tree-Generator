using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

namespace LindenmayerSystems
{
    public static class LindenmayerPlants2D
    {
        public static byte[,] Create(string axiom, TurtleRules2D turtle)
        {
            List<Vector2Int> points = new List<Vector2Int>();
            var corners = EvaluateAxiom(axiom, turtle, points);
            int resolution = NormalizePoints(points, corners.Item1, corners.Item2);
            byte[,] matrix = new byte[resolution, resolution];
            LineOnMatrix.DrawLine(matrix, points);
            return matrix;
        }

        private static Tuple<Vector2Int, Vector2Int> EvaluateAxiom(string axiom, TurtleRules2D turtle, List<Vector2Int> points)
        {
            Stack<Vector2Int> stackPositions = new Stack<Vector2Int>();
            Stack<float> stackAngles = new Stack<float>();

            stackPositions.Push(Vector2Int.zero);
            stackAngles.Push(turtle.initialRotation);

            Vector2 heading = RotateHeading(stackAngles.Peek());

            Vector2Int lower = new Vector2Int(int.MaxValue, int.MaxValue);
            Vector2Int greater = new Vector2Int(int.MinValue, int.MinValue);

            points.Add(stackPositions.Peek());
            UpdateCorners(stackPositions.Peek(), ref greater, ref lower);

            float currentAngle = 0;
            int orientationRotation = turtle.invertRotation == false ? 1 : -1;
            for (int i = 0, len = axiom.Length; i < len; i++)
            {
                switch (axiom[i])
                {
                    case 'F':
                        currentAngle = stackAngles.Pop() + turtle.tropism;
                        stackAngles.Push(currentAngle);
                        heading = RotateHeading(stackAngles.Peek());

                        var currentPosition = stackPositions.Pop();
                        stackPositions.Push(CalculateNextPosition(currentPosition, heading, turtle.LenghtMovement));
                        points.Add(stackPositions.Peek());
                        UpdateCorners(stackPositions.Peek(), ref greater, ref lower);
                        break;
                    case 'f':
                        var currentVoid = stackPositions.Pop();
                        stackPositions.Push(CalculateNextPosition(currentVoid, heading, turtle.LenghtMovement));
                        break;
                    case '+':
                        currentAngle = stackAngles.Pop() - (turtle.AngleTurn * orientationRotation);
                        stackAngles.Push(currentAngle);
                        heading = RotateHeading(stackAngles.Peek());
                        break;
                    case '-':
                        currentAngle = stackAngles.Pop() + (turtle.AngleTurn * orientationRotation);
                        stackAngles.Push(currentAngle);
                        heading = RotateHeading(stackAngles.Peek());
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
            return new Tuple<Vector2Int, Vector2Int>(greater, lower);
        }

        private static Vector2 RotateHeading(float angles)
        {
            return new Vector2(Cos(angles * Deg2Rad),
                    Sin(angles * Deg2Rad));
        }

        private static Vector2Int CalculateNextPosition(Vector2Int position, Vector2 heading, int length)
        {
            Vector2 newPosition = (Vector2)position + heading * length;
            return new Vector2Int(RoundToInt(newPosition.x), RoundToInt(newPosition.y));
        }

        private static Vector2Int CalculateNextPosition(Vector2Int position, Vector2 heading, int length, float tropism)
        {
            Vector2 newPosition = (Vector2)position + heading * length;
            return new Vector2Int(RoundToInt(newPosition.x), RoundToInt(newPosition.y));
        }

        private static void UpdateCorners(Vector2Int position, ref Vector2Int greater, ref Vector2Int lower)
        {
            lower = new Vector2Int(Min(position.x, lower.x), Min(position.y, lower.y));
            greater = new Vector2Int(Max(position.x, greater.x), Max(position.y, greater.y));
        }

        private static int NormalizePoints(List<Vector2Int> points, Vector2Int greater, Vector2Int lower)
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
    }
}