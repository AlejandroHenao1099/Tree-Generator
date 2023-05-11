using UnityEngine;

namespace MeshGenerator
{
    [System.Serializable]
    public struct SimpleSpline
    {
        private Vector3[] points;
        private float[] lenghtCurves;
        private int quantityPoints, quantityCurves;
        public int QuantityPoints { get => quantityPoints; }
        public int QuantityCurves { get => quantityCurves; }

        public SimpleSpline(Vector3[] points)
        {
            this.points = new Vector3[points.Length];
            quantityPoints = points.Length;
            quantityCurves = points.Length - 3;
            lenghtCurves = new float[points.Length];
            for (int i = 0; i < quantityPoints; i++)
                this.points[i] = points[i];

            for (int i = 0; i < quantityCurves; i++)
            {
                float currentLenght = CatmullRom.GetLengthSimpsons(0f, 1f,
                    points[i], points[i + 1], points[i + 2], points[i + 3]);
                lenghtCurves[i] = currentLenght;
            }
        }

        public Vector3 GetPoint(float t)
        {
            t = Mathf.Clamp01(t);
            if (t == 0)
                return points[1];
            else if (t == 1)
                return points[quantityPoints - 2];

            float lenghtSpline = 0f;
            for (int i = 0; i < quantityCurves; i++)
                lenghtSpline += lenghtCurves[i];

            float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;
            int indexCurve = 0;

            for (int i = 0; i < quantityCurves; i++)
            {
                if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghtCurves[i])
                {
                    indexCurve = i;
                    break;
                }
                currentLenght += lenghtCurves[i];
            }

            float d = Mathf.Abs(valueOnCurve - currentLenght);
            return CatmullRom.GetPositionWithDistance(d,
                points[indexCurve], points[indexCurve + 1], points[indexCurve + 2], points[indexCurve + 3]);
        }

        public Vector3 GetDerivative(float t)
        {
            t = Mathf.Clamp01(t);
            if (t == 0)
                return points[1];
            else if (t == 1)
                return points[quantityPoints - 2];

            float lenghtSpline = 0f;
            for (int i = 0; i < quantityCurves; i++)
                lenghtSpline += lenghtCurves[i];

            float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;
            int indexCurve = 0;

            for (int i = 0; i < quantityCurves; i++)
            {
                if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghtCurves[i])
                {
                    indexCurve = i;
                    break;
                }
                currentLenght += lenghtCurves[i];
            }

            float d = Mathf.Abs(valueOnCurve - currentLenght);
            return CatmullRom.GetDerivativeWithDistance(d,
                points[indexCurve], points[indexCurve + 1], points[indexCurve + 2], points[indexCurve + 3]);
        }

    }
}