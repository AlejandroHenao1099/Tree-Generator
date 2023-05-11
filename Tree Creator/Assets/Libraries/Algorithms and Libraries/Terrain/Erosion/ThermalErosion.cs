using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public static class ThermalErosion
    {
        private static float talus_angle = 0.0078f;
        static float magnitude = 0.5f;
        private static KernelType kernelType;

        public static void ApplyErosion(float[,] heightMap, KernelType kt, int iterations)
        {
            kernelType = kt;
            int rows = heightMap.GetLength(0);
            int cols = heightMap.GetLength(1);
            for (int pass = 0; pass < iterations; ++pass)
            {
                for (int i = 0; i < rows; ++i)
                {
                    for (int j = 0; j < cols; ++j)
                    {
                        Vector2Int center = new Vector2Int(i, j);
                        Operation(heightMap, center, Neighbours(center, rows, cols));
                    }
                }
            }
        }

        static void MoveMaterial(float[,] heightMap, Vector2Int move_from, Vector2Int move_to, float amount)
        {
            heightMap[move_from.y, move_from.x] -= amount;
            heightMap[move_to.y, move_to.x] += amount;
        }

        static void Operation(float[,] heightMap, Vector2Int center, List<Vector2Int> neighbours)
        {
            float diff_total = 0;
            float diff_max = 0;
            float center_height = heightMap[center.y, center.x];
            List<Vector2Int> lower_neighbours = new List<Vector2Int>();
            Queue<float> diffs = new Queue<float>();
            foreach (var p in neighbours)
            {
                float diff = center_height - heightMap[p.y, p.x];
                if (diff > talus_angle)
                {
                    if (diff > diff_max) diff_max = diff;
                    diff_total += diff;
                    lower_neighbours.Add(p);
                    diffs.Enqueue(diff);
                }
            }

            foreach (var p in lower_neighbours)
            {
                float diff = diffs.Dequeue();
                float amount = magnitude * (diff_max - talus_angle) * diff / diff_total;
                MoveMaterial(heightMap, center, p, amount);
            }
        }

        private static List<Vector2Int> Neighbours(Vector2Int point, int rows, int cols)
        {
            int x = point.x;
            int y = point.y;
            List<Vector2Int> points = new List<Vector2Int>();

            switch (kernelType)
            {
                case KernelType.MOORE:
                    points.Add(new Vector2Int(x - 1, y - 1));
                    points.Add(new Vector2Int(x, y - 1));
                    points.Add(new Vector2Int(x + 1, y - 1));
                    points.Add(new Vector2Int(x - 1, y));
                    points.Add(new Vector2Int(x + 1, y));
                    points.Add(new Vector2Int(x - 1, y + 1));
                    points.Add(new Vector2Int(x, y + 1));
                    points.Add(new Vector2Int(x + 1, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x - 1, y - 1), new Vector2Int(x, y - 1), new Vector2Int(x + 1, y - 1),
                    //     new Vector2Int(x - 1, y),                  new Vector2Int(x + 1, y),
                    //     new Vector2Int(x - 1, y + 1), new Vector2Int(x, y + 1), new Vector2Int(x + 1, y + 1)};
                    break;

                case KernelType.VON_NEUMANN:
                    points.Add(new Vector2Int(x, y - 1));
                    points.Add(new Vector2Int(x - 1, y));
                    points.Add(new Vector2Int(x + 1, y));
                    points.Add(new Vector2Int(x, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x, y - 1), new Vector2Int(x - 1, y),new Vector2Int(x + 1, y),
                    //            new Vector2Int(x, y + 1)};
                    break;

                case KernelType.VON_NEUMANN2:
                    points.Add(new Vector2Int(x - 1, y - 1));
                    points.Add(new Vector2Int(x + 1, y - 1));
                    points.Add(new Vector2Int(x - 1, y + 1));
                    points.Add(new Vector2Int(x + 1, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x - 1, y - 1), new Vector2Int(x + 1, y - 1),
                    //     new Vector2Int(x - 1, y + 1), new Vector2Int(x + 1, y + 1)};
                    break;
            };

            for (int it = 0; it != points.Count;)
            {
                if (!PointInRange(points[it], rows, cols))
                    points.RemoveAt(it);
                else
                    ++it;
            }
            return points;
        }

        private static bool PointInRange(Vector2Int point, int rows, int cols)
        {
            return point.x < cols && point.x >= 0 && point.y < rows && point.y >= 0;
        }
    }
}