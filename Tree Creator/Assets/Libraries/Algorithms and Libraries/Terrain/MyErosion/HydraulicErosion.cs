using System.Collections.Generic;
using UnityEngine;

namespace Erosion
{
    public static class HydraulicErosion
    {
        private static NeighbourType neighbourType;

        public static void ApplyErosion(float[,] heightMap)
        {

        }


        private static void Bilinearinterpolation(float[,] heightMap, Vector2Int center)
        {
            Vector2 gradient1 = new Vector2(heightMap[center.y, center.x + 1] - heightMap[center.y, center.x]
                 , heightMap[center.y + 1, center.x] - heightMap[center.y, center.x]);
            Vector2 gradient2 = new Vector2(heightMap[center.y, center.x + 2] - heightMap[center.y, center.x + 1]
                 , heightMap[center.y + 1, center.x + 1] - heightMap[center.y, center.x + 1]);
            Vector2 gradient3 = new Vector2(heightMap[center.y + 1, center.x + 1] - heightMap[center.y + 1, center.x]
             , heightMap[center.y + 2, center.x] - heightMap[center.y + 1, center.x]);
            Vector2 gradient4 = new Vector2(heightMap[center.y + 1, center.x + 2] - heightMap[center.y + 1, center.x + 1]
                , heightMap[center.y + 2, center.x + 1] - heightMap[center.y + 1, center.x + 1]);

            float delta = heightMap[center.y, center.x] - Mathf.Floor(heightMap[center.y, center.x]);
            
        }


        private static List<Vector2Int> Neighbours(Vector2Int point, int rows, int cols)
        {
            int x = point.x;
            int y = point.y;
            List<Vector2Int> points = new List<Vector2Int>();

            //Obtenemos los vecinos de acuerdo al Kernel
            switch (neighbourType)
            {
                case NeighbourType.Moore:
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

                case NeighbourType.VonNeumann:
                    points.Add(new Vector2Int(x, y - 1));
                    points.Add(new Vector2Int(x - 1, y));
                    points.Add(new Vector2Int(x + 1, y));
                    points.Add(new Vector2Int(x, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x, y - 1), new Vector2Int(x - 1, y),new Vector2Int(x + 1, y),
                    //            new Vector2Int(x, y + 1)};
                    break;

                case NeighbourType.VonNeumann2:
                    points.Add(new Vector2Int(x - 1, y - 1));
                    points.Add(new Vector2Int(x + 1, y - 1));
                    points.Add(new Vector2Int(x - 1, y + 1));
                    points.Add(new Vector2Int(x + 1, y + 1));
                    // points = new Vector2Int[] {
                    //     new Vector2Int(x - 1, y - 1), new Vector2Int(x + 1, y - 1),
                    //     new Vector2Int(x - 1, y + 1), new Vector2Int(x + 1, y + 1)};
                    break;
            }

            //Iteramos todos los puntos, y verificamos que esten dentro de los limites
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

    public enum NeighbourType
    {
        VonNeumann2, VonNeumann, Moore
    }

    public struct Particle
    {
        public float speed; // 1 m/s
        public float waterVolume; // 1m3
        public float sedimentVolume; // 1m3
        public Vector2Int direction;
        public int lifeTime;
    }
}