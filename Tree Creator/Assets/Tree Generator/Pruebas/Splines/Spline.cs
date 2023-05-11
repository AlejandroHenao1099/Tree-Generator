using System.Collections.Generic;
using UnityEngine;

namespace TreeCreator
{
    public class Spline
    {
        private List<Vector3> points;
        private List<float> lenghts;
        private float lenghtSpline = 0f;

        public Spline(Vector3[] points)
        {
            CreatePoints(points);
        }

        private void CreatePoints(Vector3[] newPoints)
        {
            points = new List<Vector3>();
            var p0 = newPoints[0];
            var p1 = newPoints[1];
            var newPos = p0 + (p0 - p1);
            points.Add(newPos);
            for (int i = 0; i < newPoints.Length; i++)
                points.Add(newPoints[i]);
            p0 = newPoints[newPoints.Length - 2];
            p1 = newPoints[newPoints.Length - 1];
            newPos = p1 + (p1 - p0);
            points.Add(newPos);
            CalculateLenght();
        }

        private void CalculateLenght()
        {
            lenghts = new List<float>();
            float currentLenght = 0;
            lenghts.Add(currentLenght);
            for (int i = 1; i < points.Count - 2; i++)
            {
                currentLenght += (points[i] - points[i + 1]).magnitude;
                lenghts.Add(currentLenght);
            }
            lenghtSpline = currentLenght;
        }

        public void AddPoint(Vector3 newPoint)
        {
            points[points.Count - 1] = newPoint;
            var p0 = points[points.Count - 2];
            var p1 = points[points.Count - 1];
            var newPos = p1 + (p1 - p0);
            points.Add(newPos);
            var newLenght = (p1 - p0).magnitude;
            lenghts.Add(newLenght);
            lenghtSpline += newLenght;
        }

        public Vector3 GetPoint(float t)
        {
            int i;
            if (t >= 1f)
            {
                t = 1f;
                i = points.Count - 3;
            }
            else
            {
                i = GetIndex(t);
                // t = Mathf.Clamp01(t) * CurveCount; t = 0.5
                t = Mathf.Clamp01(t) * (points.Count - 2); // t = 3.5
                int j = (int)t; // i = 3
                t -= j; // t = 0.5
                // i++;
                if (i >= points.Count - 2)
                    i = points.Count - 3;
            }
            return GetCatmullRomPosition(t, points[i - 1], points[i], points[i + 1], points[i + 2]);
        }

        private int GetIndex(float t)
        {
            t = Mathf.Clamp01(t);
            var lerpLength = Mathf.Lerp(0, lenghtSpline, t);
            for (int i = 0; i < lenghts.Count - 1; i++)
            {
                if (lerpLength >= lenghts[i] && lerpLength <= lenghts[i + 1])
                    return i + 1;
            }
            return 1;
        }

        private Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later fo
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));
            return pos;
        }

        private Vector3 GetCatmullRomDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later fo
            Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            // Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            //The cubic derivative: 0 + b + 2 * c * t + 3 * d * t^2
            Vector3 pos = 0.5f * (b + (2 * c * t) + (3 * d * t * t));
            return pos;
        }

        //Get the length of the curve with a naive method where we divide the
        //curve into straight lines and then measure the length of each line
        float GetLengthNaive(float tStart, float tEnd)
        {
            //This is the resolution, the higher the better
            int sections = 10;
            //Divide the curve into sections
            float delta = (tEnd - tStart) / (float)sections;
            //The start position of the curve
            // Vector3 lastPos = DeCasteljausAlgorithm(tStart);
            Vector3 lastPos = Vector3.zero;
            //Init length
            float length = 0f;
            //Move along the curve
            for (int i = 1; i <= sections; i++)
            {
                //Calculate the t value at this section
                float t = tStart + delta * i;
                //Find the coordinates at this t
                // Vector3 pos = DeCasteljausAlgorithm(t);
                Vector3 pos = Vector3.zero;
                //Add the section to the total length
                length += Vector3.Magnitude(pos - lastPos);
                //Save the latest pos for next loop
                lastPos = pos;
            }
            return length;
        }
    }
}