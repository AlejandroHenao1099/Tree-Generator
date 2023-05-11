using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Terrain
{
    public enum KernelType { MOORE, VON_NEUMANN, VON_NEUMANN2 }

    public static class FastErosion
    {
        private static float talus_angle = 0.0078f;
        private static KernelType kernelType;

        public static void ApplyErosion(float[,] heightMap, int iterations, KernelType typeKernel = KernelType.VON_NEUMANN2)
        {
            kernelType = typeKernel;
            for (int pass = 0; pass < iterations; ++pass)
            {
                for (int i = 0; i < heightMap.GetLength(0); ++i)
                {
                    for (int j = 0; j < heightMap.GetLength(1); ++j)
                    {
                        Vector2Int center = new Vector2Int(i, j);
                        Operation(heightMap, center, Neighbours(center, heightMap.GetLength(0), heightMap.GetLength(1)));
                    }
                }
            }
        }

        private static void Operation(float[,] heightMap, Vector2Int center, List<Vector2Int> neighbours)
        {
            float diff_max = 0;
            float center_height = heightMap[center.y, center.x];
            Vector2Int lowest_point = Vector2Int.zero;
            foreach (var p in neighbours)
            {
                float diff = center_height - heightMap[p.y, p.x];
                if (diff > diff_max)
                {
                    diff_max = diff;
                    lowest_point = p;
                }
            }

            if (0 < diff_max && diff_max >= talus_angle)
                MoveMaterial(heightMap, center, lowest_point, diff_max / 2);
        }

        private static void MoveMaterial(float[,] heightMap, Vector2Int move_from, Vector2Int move_to, float amount)
        {

            heightMap[move_from.y, move_from.x] -= amount;
            heightMap[move_to.y, move_to.x] += amount;
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