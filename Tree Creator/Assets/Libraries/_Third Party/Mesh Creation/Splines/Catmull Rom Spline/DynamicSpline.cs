using UnityEngine;

namespace MeshGenerator
{
    public struct DynamicSpline
    {
        private Vector3[] points;
        private float[] lenghtCurves;
        private int quantityPoints, quantityCurves;
        private bool loop;

        public int QuantityPoints { get => quantityPoints; }
        public int QuantityCurves { get => quantityCurves; }
        public float GetLenghtSpline() => GetLenghtAt(0);

        public void Clear()
        {
            quantityPoints = 4;
            quantityCurves = 4;
            loop = false;
            for (int i = 0; i < quantityPoints; i++)
            {
                points[i] = Vector3.zero;
                lenghtCurves[i] = 0;
            }
        }

        public bool Loop
        {
            get => loop;
            set
            {
                loop = value;
                if (value == true)
                {
                    UpdateCurve(0);
                    UpdateCurve(QuantityCurves - 2);
                    UpdateCurve(QuantityCurves - 1);
                }
            }
        }

        public Vector3 this[int index]
        {
            get
            {
                if (index >= QuantityPoints)
                    throw new System.Exception("Index fuera de rango");
                return points[index];
            }
        }

        public float GetLenght(int index)
        {
            if (index < 0 || index >= QuantityPoints)
                return 0f;
            return lenghtCurves[index];
        }

        public DynamicSpline(Vector3[] points)
        {
            int lenghtPoints = points.Length < 16 ? 16 : points.Length < 24 ? 24 : points.Length;
            this.points = new Vector3[lenghtPoints];
            quantityPoints = points.Length;
            quantityCurves = points.Length;
            lenghtCurves = new float[lenghtPoints];
            loop = false;
            for (int i = 0; i < quantityPoints; i++)
                this.points[i] = points[i];
            for (int i = 0; i < quantityCurves; i++)
                UpdateCurve(i);
        }

        public void AddPoint(Vector3 newPoint, bool updateSpline = false)
        {
            if (quantityPoints + 1 >= points.Length)
            {
                int newSize = points.Length + 8;
                System.Array.Resize(ref points, newSize);
                System.Array.Resize(ref lenghtCurves, newSize);
            }
            points[quantityPoints] = newPoint;
            quantityPoints++;
            quantityCurves++;

            if (Loop == false)
                UpdateCurve(QuantityCurves - 3);
            else
            {
                UpdateCurve(QuantityCurves - 3);
                UpdateCurve(QuantityCurves - 2);
                UpdateCurve(QuantityCurves - 1);
                UpdateCurve(0);
            }
        }

        public void IncreaseResolution(int increment)
        {
            if (quantityPoints + increment >= points.Length)
            {
                int incrementSize = increment < 8 ? 8 : increment;
                int newSize = points.Length + incrementSize;
                System.Array.Resize(ref points, newSize);
                System.Array.Resize(ref lenghtCurves, newSize);
            }

            var desiredSize = quantityPoints + increment;
            var newPoints = new Vector3[desiredSize];
            float step = Loop == true ? 1f / (desiredSize) : 1f / (desiredSize - 3);

            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? desiredSize : desiredSize - 2;

            for (int i = 0; i < endIndex; i++)
                newPoints[startIndex + i] = GetPoint(step * i);

            quantityPoints += increment;
            quantityCurves += increment;


            if (Loop == false)
                points[quantityPoints - 1] = points[quantityPoints - 1 - increment];

            endIndex = Loop == false ? endIndex + 1 : QuantityPoints;
            for (int i = startIndex; i < endIndex; i++)
                points[i] = newPoints[i];

            UpdateSpline();
        }

        public bool RemovePoint(int index, bool updateSpline = false)
        {
            if (quantityPoints == 4 || index >= quantityPoints || index < 0)
                return false;

            for (int i = index; i < quantityPoints - 1; i++)
            {
                points[i] = points[i + 1];
                lenghtCurves[i] = lenghtCurves[i + 1];
            }

            quantityPoints--;
            quantityCurves--;

            if (updateSpline == false)
                return true;

            if (Loop == false)
            {
                if (index <= 1)
                {
                    UpdateCurve(1);
                    return true;
                }
                else if (index >= quantityPoints - 1)
                {
                    UpdateCurve(QuantityCurves - 3);
                    return true;
                }
            }
            // index = index - 1 < 0 ? index - 1 + QuantityCurves - 1 : index - 1;
            index = (index + QuantityCurves - 1) % QuantityCurves;
            GetIndexCurve(index, out int a, out int b, out int c, out int d);
            UpdateCurve(a);
            UpdateCurve(b);
            UpdateCurve(c);
            UpdateCurve(d);
            return true;
        }

        public bool UpdatePoint(Vector3 newPoint, int index, bool updateSpline = false)
        {
            if (index >= quantityPoints || index < 0)
                return false;

            points[index] = newPoint;

            if (updateSpline == false)
                return true;


            if (Loop == false)
            {
                if (index <= 1)
                {
                    UpdateCurve(1);
                    return true;
                }
                else if (index >= quantityPoints - 2)
                {
                    UpdateCurve(QuantityCurves - 3);
                    return true;
                }
            }
            GetIndexCurve(index - 1, out int a, out int b, out int c, out int d);

            UpdateCurve(a);
            UpdateCurve(b);
            UpdateCurve(c);
            UpdateCurve(d);
            return true;
        }

        public void UpdateSpline()
        {
            for (int i = 0; i < QuantityCurves; i++)
                UpdateCurve(i);
        }

        private void UpdateCurve(int indexCurve)
        {
            if (indexCurve < 0 || indexCurve >= QuantityCurves)
                throw new System.Exception("Fuera de Rango");

            GetIndexCurve(indexCurve, out int p0, out int p1, out int p2, out int p3);

            float currentLenght = CatmullRom.GetLengthSimpsons(0f, 1f,
                points[p0], points[p1], points[p2], points[p3]);
            lenghtCurves[indexCurve] = currentLenght;
        }

        public Vector3 GetPoint(float t)
        {
            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? 0 : quantityPoints - 2;

            if (t <= 0)
                return points[startIndex];
            else if (t >= 1)
                return points[endIndex];

            t = Mathf.Clamp01(t);

            // startIndex = Loop == true ? 0 : 1;
            endIndex = Loop == true ? QuantityCurves : QuantityCurves - 2;

            float lenghtSpline = 0f;
            for (int i = startIndex; i < endIndex; i++)
                lenghtSpline += lenghtCurves[i];

            float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;
            int indexCurve = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghtCurves[i])
                {
                    indexCurve = i;
                    break;
                }
                currentLenght += lenghtCurves[i];
            }

            GetIndexCurve(indexCurve, out int p0, out int p1, out int p2, out int p3);
            float d = Mathf.Abs(valueOnCurve - currentLenght);
            return CatmullRom.GetPositionWithDistance(d, points[p0], points[p1], points[p2], points[p3]);
        }

        public void GetIndexCurve(int indexCurve, out int p0, out int p1, out int p2, out int p3)
        {
            p0 = (indexCurve + QuantityCurves - 1) % QuantityCurves;
            p1 = indexCurve % QuantityCurves;
            p2 = (indexCurve + 1) % QuantityCurves;
            p3 = (indexCurve + 2) % QuantityCurves;
        }

        public Vector3 GetDerivative(float t)
        {
            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? QuantityCurves : QuantityCurves - 2;

            float lenghtSpline = 0f;
            for (int i = startIndex; i < endIndex; i++)
                lenghtSpline += lenghtCurves[i];

            t = t >= 1 ? 0.999999f : Mathf.Clamp01(t);
            float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;
            int indexCurve = 0;

            for (int i = startIndex; i < endIndex; i++)
            {
                if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghtCurves[i])
                {
                    indexCurve = i;
                    break;
                }
                currentLenght += lenghtCurves[i];
            }

            float d = Mathf.Abs(valueOnCurve - currentLenght);
            GetIndexCurve(indexCurve, out int p0, out int p1, out int p2, out int p3);

            return CatmullRom.GetDerivativeWithDistance(d,
                points[p0], points[p1], points[p2], points[p3]).normalized;
        }

        public Vector3 GetNormal(float t, Vector3 initialNormal)
        {
            t = Mathf.Clamp01(t);
            var walkerSpline = new WalkerSpline(GetDerivative(0f), initialNormal);
            float step = t / 100f;
            for (int i = 1; i <= 100; i++)
                walkerSpline.UpdateDirection(GetDerivative(i * step));
            return walkerSpline.GetPoint(Vector3.up).normalized;
        }

        public float GetLenghtAt(float t)
        {
            if (t <= 0)
                return 0f;

            float lenghtSpline = 0f;
            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? QuantityCurves : QuantityCurves - 2;
            for (int i = startIndex; i < endIndex; i++)
                lenghtSpline += lenghtCurves[i];

            if (t >= 1)
                return lenghtSpline;

            t = Mathf.Clamp01(t);
            return Mathf.Lerp(0, lenghtSpline, t);
        }

        public Vector3 GetCylindricalCoordinates(float normalizedPosition, float angle,
                Vector3 initialUp, float radius = 1f)
        {
            normalizedPosition = Mathf.Clamp01(normalizedPosition);
            var walkerSpline = new WalkerSpline(GetDerivative(0), initialUp);
            if (normalizedPosition.Equals(0))
                return walkerSpline.GetCylindricalPoint(angle, radius);

            int quantitySteps = Mathf.RoundToInt(Mathf.Lerp(0, quantityPoints - 1, normalizedPosition));
            float stepRes = normalizedPosition / (float)(quantitySteps);

            for (int i = 0; i <= quantitySteps; i++)
            {
                var currentPoint = GetPoint(i * stepRes);
                walkerSpline.UpdatePosition(currentPoint);
                var currentDirection = GetDerivative(i * stepRes);
                walkerSpline.UpdateDirection(currentDirection);
            }
            return walkerSpline.GetCylindricalPoint(angle, radius);
        }

        // public void GetIndexAt(float t, out int index, out float lerp, out float distance)
        // {
        //     int startIndex = Loop == true ? 0 : 1;
        //     int endIndex = Loop == true ? 0 : quantityPoints - 2;

        //     if (t <= 0)
        //     {
        //         index = startIndex;
        //         lerp = 0f;
        //         distance = 0f;
        //         return;
        //     }

        //     t = Mathf.Clamp01(t);

        //     startIndex = Loop == true ? 0 : 1;
        //     endIndex = Loop == true ? QuantityCurves : QuantityCurves - 2;

        //     float lenghtSpline = 0f;
        //     for (int i = startIndex; i < endIndex; i++)
        //         lenghtSpline += lenghtCurves[i];

        //     float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);

        //     float currentLenght = 0f;
        //     index = 0;

        //     for (int i = startIndex; i < endIndex; i++)
        //     {
        //         if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghtCurves[i])
        //         {
        //             index = i;
        //             break;
        //         }
        //         currentLenght += lenghtCurves[i];
        //     }
        //     distance = Mathf.Abs(valueOnCurve - currentLenght);
        //     lerp = Mathf.InverseLerp(0f, lenghtCurves[])

        //     // GetIndexCurve(indexCurve, out int p0, out int p1, out int p2, out int p3);
        //     // return CatmullRom.GetPositionWithDistance(d, points[p0], points[p1], points[p2], points[p3]);
        // }

    }
}