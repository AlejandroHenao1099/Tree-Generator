using System.Collections.Generic;
using UnityEngine;

namespace ProceduralMeshGeneration
{
    public static class SpaceGenerator
    {
        public static Vector3 GetPoint(Vector3 normalizePoint, SpaceType axisX, SpaceType axisY, SpaceType axisZ)
        {
            normalizePoint.x = Mathf.Clamp(normalizePoint.x, -1f, 1f);
            normalizePoint.y = Mathf.Clamp(normalizePoint.y, -1f, 1f);
            normalizePoint.z = Mathf.Clamp(normalizePoint.z, -1f, 1f);
            float x = GetTransformedValue(normalizePoint.x, axisX);
            float y = GetTransformedValue(normalizePoint.y, axisY);
            float z = GetTransformedValue(normalizePoint.z, axisZ);
            return new Vector3(x, y, z);

            // switch (axisX)
            // {
            //     case SpaceType.Lineal:
            //         return normalizePoint;
            //     case SpaceType.Quadratic:
            //         return new Vector3(normalizePoint.x * normalizePoint.x, normalizePoint.y * normalizePoint.y,
            //             normalizePoint.z * normalizePoint.z);
            //     case SpaceType.Cubic:
            //         return new Vector3(normalizePoint.x * normalizePoint.x * normalizePoint.x,
            //             normalizePoint.y * normalizePoint.y * normalizePoint.y,
            //             normalizePoint.z * normalizePoint.z * normalizePoint.z);
            //     case SpaceType.SquareRoot:
            //         return new Vector3(
            //             Mathf.Sqrt(Mathf.Abs(normalizePoint.x)),
            //             Mathf.Sqrt(Mathf.Abs(normalizePoint.y)),
            //             Mathf.Sqrt(Mathf.Abs(normalizePoint.z))
            //             );
            //     case SpaceType.SemiCircle:
            //         return new Vector3(
            //             Mathf.Sqrt(1 - normalizePoint.x * normalizePoint.x),
            //             Mathf.Sqrt(1 - normalizePoint.y * normalizePoint.y),
            //             Mathf.Sqrt(1 - normalizePoint.z * normalizePoint.z)
            //             );
            //     case SpaceType.Cos:
            //         return new Vector3(
            //             Mathf.Cos(normalizePoint.x * Mathf.PI * 2f),
            //             Mathf.Cos(normalizePoint.y * Mathf.PI * 2f),
            //             Mathf.Cos(normalizePoint.z * Mathf.PI * 2f)
            //             );
            //     case SpaceType.Sin:
            //         return new Vector3(
            //             Mathf.Sin(normalizePoint.x * Mathf.PI * 2f),
            //             Mathf.Sin(normalizePoint.y * Mathf.PI * 2f),
            //             Mathf.Sin(normalizePoint.z * Mathf.PI * 2f)
            //             );
            //     default:
            //         return normalizePoint;
            // }
        }

        public static float GetTransformedValue(float normalizeValue, SpaceType spaceType)
        {
            switch (spaceType)
            {
                case SpaceType.Lineal:
                    return normalizeValue;
                case SpaceType.Quadratic:
                    return normalizeValue * normalizeValue;
                case SpaceType.Cubic:
                    return normalizeValue * normalizeValue * normalizeValue;
                case SpaceType.SquareRoot:
                    return Mathf.Sqrt(Mathf.Abs(normalizeValue));
                case SpaceType.SemiCircle:
                    return Mathf.Sqrt(1 - normalizeValue * normalizeValue);
                case SpaceType.Cos:
                    return Mathf.Cos(normalizeValue * Mathf.PI * 2f);
                case SpaceType.Sin:
                    return Mathf.Sin(normalizeValue * Mathf.PI * 2f);
                default:
                    return normalizeValue;
            }
        }
    }
}