using UnityEngine;

namespace MeshGenerator
{
    public struct DynamicLine
    {
        private Vector3[] points;
        private float[] lenghtSegments;
        private int quantityPoints, quantitySegments;
        private bool loop;

        public int QuantityPoints { get => quantityPoints; }
        public int QuantitySegments { get => quantitySegments; }
        public float GetLenghtLine() => GetLenghtAt(0);

        public bool Loop
        {
            get => loop;

            set
            {
                loop = value;
                if (value == true)
                    UpdateSegment(QuantitySegments - 1);
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
            return lenghtSegments[index];
        }

        public DynamicLine(Vector3[] points)
        {
            int lenghtPoints = points.Length < 16 ? 16 : points.Length < 24 ? 24 : points.Length;
            this.points = new Vector3[lenghtPoints];
            quantityPoints = points.Length;
            quantitySegments = points.Length;
            lenghtSegments = new float[lenghtPoints];
            loop = false;
            for (int i = 0; i < quantityPoints; i++)
                this.points[i] = points[i];
            for (int i = 0; i < quantitySegments; i++)
                UpdateSegment(i);
        }

        public void AddPoint(Vector3 newPoint, bool updateLine = false)
        {
            if (quantityPoints + 1 >= points.Length)
            {
                int newSize = points.Length + 8;
                System.Array.Resize(ref points, newSize);
                System.Array.Resize(ref lenghtSegments, newSize);
            }
            points[quantityPoints] = newPoint;
            quantityPoints++;
            quantitySegments++;

            UpdateSegment(QuantitySegments - 2);
            UpdateSegment(QuantitySegments - 1);
        }

        public void IncreaseResolution(int increment)
        {
            if (quantityPoints + increment >= points.Length)
            {
                int incrementSize = increment < 8 ? 8 : increment;
                int newSize = points.Length + incrementSize;
                System.Array.Resize(ref points, newSize);
                System.Array.Resize(ref lenghtSegments, newSize);
            }

            var desiredSize = QuantityPoints + increment;
            var newPoints = new Vector3[desiredSize];
            float step = Loop == true ? 1f / (desiredSize) : 1f / (desiredSize - 1);

            int startIndex = 0;
            int endIndex = desiredSize;

            for (int i = startIndex; i < endIndex; i++)
                newPoints[i] = GetPoint(step * i);

            quantityPoints += increment;
            quantitySegments += increment;

            for (int i = startIndex; i < endIndex; i++)
                points[i] = newPoints[i];

            UpdateLine();
        }

        public bool RemovePoint(int index, bool updateLine = false)
        {
            if (quantityPoints == 4 || index >= quantityPoints || index < 0)
                return false;

            for (int i = index; i < quantityPoints - 1; i++)
            {
                points[i] = points[i + 1];
                lenghtSegments[i] = lenghtSegments[i + 1];
            }

            quantityPoints--;
            quantitySegments--;

            if (updateLine == false)
                return true;

            index = (index + QuantityPoints - 1) % QuantityPoints;
            UpdateSegment(index);
            return true;
        }

        public bool UpdatePoint(Vector3 newPoint, int index, bool updateLine = false)
        {
            if (index >= quantityPoints || index < 0)
                return false;

            points[index] = newPoint;

            if (updateLine == false)
                return true;

            int prevIndex = (index + QuantityPoints - 1) % QuantityPoints;

            UpdateSegment(prevIndex);
            UpdateSegment(index);
            return true;
        }

        public void UpdateLine()
        {
            for (int i = 0; i < QuantitySegments; i++)
                UpdateSegment(i);
        }

        private void UpdateSegment(int indexSegment)
        {
            if (indexSegment < 0 || indexSegment >= QuantitySegments)
                throw new System.Exception("Fuera de Rango");

            int nextIndex = (indexSegment + 1) % QuantitySegments;
            lenghtSegments[indexSegment] = (points[indexSegment] - points[nextIndex]).magnitude;
        }

        public Vector3 GetPoint(float t)
        {
            GetIndexAt(t, out int index, out float lerp);
            // Debug.Log("T: " + t + " Index: " + index + " Lerp: " + lerp);
            return GetPointAt(index, lerp);
            // int endIndex = Loop == true ? 0 : QuantityPoints - 1;

            // if (t <= 0)
            //     return points[0];
            // else if (t >= 1)
            //     return points[endIndex];

            // t = Mathf.Clamp01(t);
            // endIndex = Loop == true ? QuantityPoints : QuantityPoints - 1;

            // float lenghtSpline = 0f;
            // for (int i = 0; i < endIndex; i++)
            //     lenghtSpline += lenghtSegments[i];

            // float valueOnLine = Mathf.Lerp(0, lenghtSpline, t);

            // float currentLenght = 0f;
            // int indexCurve = 0;

            // for (int i = 0; i < endIndex; i++)
            // {
            //     if (valueOnLine >= currentLenght && valueOnLine <= currentLenght + lenghtSegments[i])
            //     {
            //         indexCurve = i;
            //         break;
            //     }
            //     currentLenght += lenghtSegments[i];
            // }
            // float lerp = Mathf.InverseLerp(currentLenght, currentLenght + lenghtSegments[indexCurve], valueOnLine);

            // int next = (indexCurve + 1) % QuantityPoints;
            // return Vector3.Lerp(points[indexCurve], points[next], lerp);
        }

        public Vector3 GetDerivative(float t)
        {
            GetIndexAt(t, out int index, out float lerp);
            int next = (index + 1) % QuantityPoints;
            return (points[next] - points[index]).normalized;

            // if (t <= 0)
            //     return (points[1] - points[0]).normalized;
            // else if (t >= 1)
            //     return (points[QuantityPoints - 1] - points[QuantityPoints - 2]).normalized;

            // int endIndex = Loop == true ? QuantitySegments : QuantitySegments - 1;
            // t = Mathf.Clamp01(t);
            // float lenghtLine = 0f;
            // for (int i = 0; i < endIndex; i++)
            //     lenghtLine += lenghtSegments[i];

            // float valueOnLine = Mathf.Lerp(0, lenghtLine, t);

            // float currentLenght = 0f;
            // int indexCurve = 0;

            // for (int i = 0; i < endIndex; i++)
            // {
            //     if (valueOnLine >= currentLenght && valueOnLine <= currentLenght + lenghtSegments[i])
            //     {
            //         indexCurve = i;
            //         break;
            //     }
            //     currentLenght += lenghtSegments[i];
            // }
            // int next = (indexCurve + 1) % QuantityPoints;
            // return (points[next] - points[indexCurve]).normalized;
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
            t = Mathf.Clamp01(t);
            if (t <= 0)
                return 0f;

            float lenghtSpline = 0f;
            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? QuantitySegments : QuantitySegments - 2;
            for (int i = startIndex; i < endIndex; i++)
                lenghtSpline += lenghtSegments[i];

            if (t >= 1)
                return lenghtSpline;

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

        public void GetIndexAt(float t, out int index, out float lerp)
        {
            int endIndex = Loop == true ? 0 : QuantityPoints - 1;

            if (t <= 0)
            {
                lerp = 0f;
                index = 0;
                return;
            }
            else if (t >= 1)
            {
                lerp = 0f;
                index = endIndex;
                return;
            }

            t = Mathf.Clamp01(t);
            endIndex = Loop == true ? QuantityPoints : QuantityPoints - 1;
            index = 0;

            float lenghtSpline = 0f;
            for (int i = 0; i < endIndex; i++)
                lenghtSpline += lenghtSegments[i];

            float valueOnLine = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;

            for (int i = 0; i < endIndex; i++)
            {
                if (valueOnLine >= currentLenght && valueOnLine <= currentLenght + lenghtSegments[i])
                {
                    index = i;
                    break;
                }
                currentLenght += lenghtSegments[i];
            }
            lerp = Mathf.InverseLerp(currentLenght, currentLenght + lenghtSegments[index], valueOnLine);

            // int endIndex = Loop == true ? 0 : QuantityPoints - 1;

            // if (t <= 0)
            //     return points[0];
            // else if (t >= 1)
            //     return points[endIndex];

            // t = Mathf.Clamp01(t);
            // endIndex = Loop == true ? QuantityPoints : QuantityPoints - 1;

            // float lenghtSpline = 0f;
            // for (int i = 0; i < endIndex; i++)
            //     lenghtSpline += lenghtSegments[i];

            // float valueOnLine = Mathf.Lerp(0, lenghtSpline, t);

            // float currentLenght = 0f;
            // int indexCurve = 0;

            // for (int i = 0; i < endIndex; i++)
            // {
            //     if (valueOnLine >= currentLenght && valueOnLine <= currentLenght + lenghtSegments[i])
            //     {
            //         indexCurve = i;
            //         break;
            //     }
            //     currentLenght += lenghtSegments[i];
            // }
            // float lerp = Mathf.InverseLerp(currentLenght, currentLenght + lenghtSegments[indexCurve], valueOnLine);

            // int next = (indexCurve + 1) % QuantityPoints;
            // return Vector3.Lerp(points[indexCurve], points[next], lerp);
        }

        public Vector3 GetPointAt(int index, float lerp)
        {
            if (index >= QuantityPoints || index < 0)
                throw new System.Exception("Index No Valido");
            int next = (index + 1) % QuantityPoints;
            return Vector3.Lerp(points[index], points[next], lerp);
        }
    }
}