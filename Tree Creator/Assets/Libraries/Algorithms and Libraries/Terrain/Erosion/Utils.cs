using System.Collections.Generic;

namespace Terrain
{
    public static class Utils
    {
        public static float Average(List<float> vec)
        {
            float retValue = 0;
            foreach (var item in vec)
                retValue += item;
            return retValue / vec.Count;
        }

        public static float Sum(List<float> vec)
        {
            float retValue = 0;
            foreach (var item in vec)
                retValue += item;
            return retValue;
        }

        public static bool PointInRange(int r, int c, int r_max, int c_max)
        {
            return (r >= 0) && (r < r_max) && (c >= 0) && (c < c_max);
        }
    }
}