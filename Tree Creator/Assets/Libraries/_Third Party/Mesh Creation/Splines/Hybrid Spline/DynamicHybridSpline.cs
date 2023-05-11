using UnityEngine;

namespace MeshGenerator
{
    [System.Serializable]
    public struct DynamicHybridSpline
    {
        private DynamicSpline spline;
        private DynamicLine line;

        private StateSegment[] segments;
        private float[] lenghtSegments;
        private int quantityPoints;


        public bool Loop
        {
            get => spline.Loop;
            set
            {
                spline.Loop = value;
                line.Loop = value;
            }
        }

        public DynamicHybridSpline(Vector3[] points)
        {
            spline = new DynamicSpline(points);
            line = new DynamicLine(points);

            int lenghtPoints = points.Length < 16 ? 16 : points.Length < 24 ? 24 : points.Length;
            segments = new StateSegment[lenghtPoints];
            lenghtSegments = new float[lenghtPoints];
            quantityPoints = points.Length;
            UpdateLenghtSegments();
        }

        public void AddPoint(Vector3 newPoint, StateSegment state = StateSegment.Curve, bool updateSpline = false)
        {
            if (quantityPoints + 1 >= segments.Length)
            {
                int newSize = segments.Length + 8;
                System.Array.Resize(ref segments, newSize);
                System.Array.Resize(ref lenghtSegments, newSize);
            }

            spline.AddPoint(newPoint, updateSpline);
            line.AddPoint(newPoint, updateSpline);

            segments[quantityPoints] = state;
            lenghtSegments[quantityPoints] = 0f;
            quantityPoints++;
            if(updateSpline)
                UpdateLenghtSegments();
        }

        public void IncreaseResolution(int increment)
        {
            if (quantityPoints + increment >= segments.Length)
            {
                int incrementSize = increment < 8 ? 8 : increment;
                int newSize = segments.Length + incrementSize;
                System.Array.Resize(ref segments, newSize);
                System.Array.Resize(ref lenghtSegments, newSize);
            }

            spline.IncreaseResolution(increment);
            line.IncreaseResolution(increment);

            for (int i = 0; i < increment; i++)
            {
                segments[quantityPoints + i] = (byte)StateSegment.Curve;
                lenghtSegments[quantityPoints + i] = (byte)StateSegment.Curve;
            }
            quantityPoints += increment;
            UpdateLenghtSegments();
        }

        public bool RemovePoint(int index, bool updateLine = false)
        {
            if (quantityPoints == 4 || index >= quantityPoints || index < 0)
                return false;

            for (int i = index; i < quantityPoints - 1; i++)
            {
                segments[i] = segments[i + 1];
                lenghtSegments[i] = lenghtSegments[i + 1];
            }

            quantityPoints--;
            spline.RemovePoint(index, updateLine);
            line.RemovePoint(index, updateLine);
            return true;
        }

        public bool UpdatePoint(Vector3 newPoint, int index, bool updateLine = false)
        {
            if (index >= quantityPoints || index < 0)
                return false;

            spline.UpdatePoint(newPoint, index, updateLine);
            line.UpdatePoint(newPoint, index, updateLine);
            switch (segments[index])
            {
                case StateSegment.Line:
                    lenghtSegments[index] = line.GetLenght(index);
                    return true;
                default:
                    lenghtSegments[index] = spline.GetLenght(index);
                    return true;
            }
            UpdateLenghtSegments();
        }

        public bool UpdateStateSegment(int index, StateSegment newState)
        {
            if (index >= quantityPoints || index < 0)
                return false;

            segments[index] = newState;
            switch (newState)
            {
                case StateSegment.Line:
                    lenghtSegments[index] = line.GetLenght(index);
                    break;
                default:
                    lenghtSegments[index] = spline.GetLenght(index);
                    break;
            }
            return true;
        }

        public void Update()
        {
            spline.UpdateSpline();
            line.UpdateLine();
            UpdateLenghtSegments();
        }

        public Vector3 GetPoint(float t)
        {
            // int startIndex = segments[0] == StateSegment.Line ? 0 : Loop == true ? 0 : 1;
            // int endIndex = Loop == true ? 0 : quantityPoints - 1;
            // endIndex = segments[endIndex] == StateSegment.Line ? endIndex : quantityPoints - 2;

            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? 0 : quantityPoints - 2;

            // if (segments[0] == StateSegment.Line || Loop == true)
            //     startIndex = 0;
            // else if (Loop == false && segments[0] == StateSegment.Curve)
            //     startIndex = 1;
            
            // if(Loop == true)
            //     endIndex = quantityPoints;
            // else if(segments[quantityPoints - 1] == StateSegment.Line)
            //     endIndex = quantityPoints - 1;

            if (t <= 0)
                return line[startIndex];
            else if (t >= 1)
                return line[endIndex];

            t = Mathf.Clamp01(t);

            // startIndex = Loop == true ? 0 : 1;
            // endIndex = Loop == true ? quantityPoints : quantityPoints - 1;
            // if (Loop == true)
            // {
            //     endIndex = quantityPoints;
            // }
            // else if (segments[quantityPoints - 1] == StateSegment.Curve)
            //     endIndex = quantityPoints - 1;
            // else
            //     endIndex = quantityPoints - 2;
            startIndex = Loop == true ? 0 : 1;
            endIndex = Loop == true ? quantityPoints : quantityPoints - 2;

            float lenghtSpline = 0f;
            for (int i = 0; i < endIndex; i++)
                lenghtSpline += lenghtSegments[i];

            float valueOnLine = Mathf.Lerp(0, lenghtSpline, t);

            float currentLenght = 0f;
            int indexSegment = 0;

            for (int i = 0; i < endIndex; i++)
            {
                if (valueOnLine >= currentLenght && valueOnLine <= currentLenght + lenghtSegments[i])
                {
                    indexSegment = i;
                    break;
                }
                currentLenght += lenghtSegments[i];
            }

            int next = (indexSegment + 1) % quantityPoints;

            if (segments[indexSegment] == StateSegment.Line)
            {
                float lerp = Mathf.InverseLerp(currentLenght, currentLenght + lenghtSegments[indexSegment], valueOnLine);
                return Vector3.Lerp(line[indexSegment], line[next], lerp);
            }
            spline.GetIndexCurve(indexSegment, out int p0, out int p1, out int p2, out int p3);
            float d = Mathf.Abs(valueOnLine - currentLenght);

            Debug.Log("D: " + d + " P0: " + p0 + " P!:" + p1 + " P2: " + p2 + " P3: " + p3);
            var ret = CatmullRom.GetPositionWithDistance(d, spline[p0], spline[p1], spline[p2], spline[p3]);
            // Debug.Log("T: " + t + " Pos: " + ret);
            return ret;
            // return CatmullRom.GetPositionWithDistance(d, spline[p0], spline[p1], spline[p2], spline[p3]);

            // line.GetIndexAt(t, out int index, out float lerp);
            // return GetHybridPoint(index, lerp, t, (StateSegment)segments[index]);
        }

        private Vector3 GetHybridPoint(int index, float lerp, float t, StateSegment stateSegment)
        {
            switch (stateSegment)
            {
                case StateSegment.Curve:
                    return spline.GetPoint(t);
                case StateSegment.Line:
                    return line.GetPointAt(index, lerp);
                default:
                    return spline.GetPoint(lerp);
            }
        }

        public Vector3 GetDerivative(float t)
        {
            line.GetIndexAt(t, out int index, out float lerp);
            return GetHybridDerivative(index, lerp, t, (StateSegment)segments[index]);
        }

        private Vector3 GetHybridDerivative(int index, float lerp, float t, StateSegment stateSegment)
        {
            switch (stateSegment)
            {
                case StateSegment.Curve:
                    return spline.GetDerivative(t);
                case StateSegment.Line:
                    return line.GetDerivative(t);
                default:
                    return spline.GetDerivative(t);
            }
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

            float lenght = 0f;
            int startIndex = Loop == true ? 0 : 1;
            int endIndex = Loop == true ? quantityPoints : quantityPoints - 2;
            for (int i = startIndex; i < endIndex; i++)
                lenght += lenghtSegments[i];

            if (t >= 1)
                return lenght;

            return Mathf.Lerp(0, lenght, t);
        }

        private float GetLenghtAt(int index)
        {
            switch ((StateSegment)segments[index])
            {
                case StateSegment.Line:
                    return line.GetLenght(index);
                default:
                    return spline.GetLenght(index);
            }
        }

        private void UpdateLenghtSegments()
        {
            for (int i = 0; i < quantityPoints; i++)
                lenghtSegments[i] = GetLenghtAt(i);
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
    }
}

public enum StateSegment
{
    Curve, Line
}