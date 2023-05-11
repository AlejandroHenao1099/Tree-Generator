using UnityEngine;

namespace TreeCreator
{
    public static class TreeUtils
    {
        public static int RandomSign()
        {
            float randomValue = Random.Range(0f, 1f) * 2f - 2f;
            return Mathf.RoundToInt(Mathf.Sign(randomValue));
        }

        public static float ShapeRatio(Shape shape, float ratio)
        {
            ratio = Mathf.Clamp01(ratio);
            switch (shape)
            {
                case Shape.Conical:
                    return 0.2f + 0.8f * ratio;
                case Shape.Spherical:
                    return 0.2f + 0.8f * Mathf.Sin(ratio * Mathf.PI);
                case Shape.Hemispherical:
                    return 0.2f + 0.8f * Mathf.Sin(ratio * Mathf.PI * 0.5f);
                case Shape.Cylindrical:
                    return 1f;
                case Shape.TaperedCylindrical:
                    return 0.5f + 0.5f * ratio;
                case Shape.Flame:
                    return ratio <= 0.7f ? ratio / 0.7f : (1.0f - ratio) / 0.3f;
                case Shape.InverseConical:
                    return 1 - 0.8f * ratio;
                case Shape.TendFlame:
                    return ratio <= 0.7f ? 0.5f + 0.5f * ratio / 0.7f : 0.5f + 0.5f * (1 - ratio) / 0.3f;
                case Shape.Envelope:
                    return 1;
                default:
                    return 1;
            }
        }

    }

    public enum ShapeCurve
    {
        Default, Spiral
    }

    public enum Shape
    {
        Conical, Spherical, Hemispherical, Cylindrical, TaperedCylindrical, Flame, InverseConical, TendFlame,
        Envelope
    }
}