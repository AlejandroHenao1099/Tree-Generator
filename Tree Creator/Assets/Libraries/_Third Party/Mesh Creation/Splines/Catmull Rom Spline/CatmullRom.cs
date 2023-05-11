using UnityEngine;

namespace MeshGenerator
{
    public static class CatmullRom
    {
        public static Vector3 GetPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float totalLength = GetLengthSimpsons(0f, 1f, p0, p1, p2, p3);
            float lerpDistance = Mathf.Lerp(0, totalLength, t);
            float realT = FindTValue(lerpDistance, totalLength, p0, p1, p2, p3);
            return GetCatmullRomPosition(realT, p0, p1, p2, p3);
        }

        public static Vector3 GetPositionWithDistance(float distance, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float totalLength = GetLengthSimpsons(0f, 1f, p0, p1, p2, p3);
            float realT = FindTValue(distance, totalLength, p0, p1, p2, p3);
            return GetCatmullRomPosition(realT, p0, p1, p2, p3);
        }

        public static Vector3 GetDerivativeWithDistance(float distance, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float totalLength = GetLengthSimpsons(0f, 1f, p0, p1, p2, p3);
            float realT = FindTValue(distance, totalLength, p0, p1, p2, p3);
            return GetCatmullRomDerivative(realT, p0, p1, p2, p3);
        }

        public static Vector3 GetDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float totalLength = GetLengthSimpsons(0f, 1f, p0, p1, p2, p3);
            float lerpDistance = Mathf.Lerp(0, totalLength, t);
            float realT = FindTValue(lerpDistance, totalLength, p0, p1, p2, p3);
            return GetCatmullRomDerivative(realT, p0, p1, p2, p3);
        }

        private static Vector3 GetCatmullRomPosition(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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

        private static Vector3 GetCatmullRomDerivative(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The coefficients of the cubic polynomial (except the 0.5f * which I added later fo
            // Vector3 a = 2f * p1;
            Vector3 b = p2 - p0;
            Vector3 c = 2f * p0 - 5f * p1 + 4f * p2 - p3;
            Vector3 d = -p0 + 3f * p1 - 3f * p2 + p3;
            //The cubic polynomial: a + b * t + c * t^2 + d * t^3
            // Vector3 pos = 0.5f * (a + (b * t) + (c * t * t) + (d * t * t * t));

            //The cubic derivative: 0 + b + 2 * c * t + 3 * d * t^2
            Vector3 der = 0.5f * (b + (2 * c * t) + (3 * d * t * t));
            return der;
        }

        //Use Newton–Raphsons method to find the t value at the end of this distance d
        private static float FindTValue(float d, float totalLength, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //Need a start value to make the method start
            //Should obviously be between 0 and 1
            //We can say that a good starting point is the percentage of distance traveled
            //If this start value is not working you can use the Bisection Method to find a start value
            //https://en.wikipedia.org/wiki/Bisection_method
            float t = d / totalLength;
            //Need an error so we know when to stop the iteration
            float error = 0.001f;
            //We also need to avoid infinite loops
            int iterations = 0;
            while (true)
            {
                //Newton's method
                float tNext = t - ((GetLengthSimpsons(0f, t, p0, p1, p2, p3) - d) / GetArcLengthIntegrand(t, p0, p1, p2, p3));
                //Have we reached the desired accuracy?
                if (Mathf.Abs(tNext - t) < error)
                {
                    break;
                }
                t = tNext;
                iterations += 1;
                if (iterations > 1000)
                {
                    break;
                }
            }
            return t;
        }

        public static float GetLengthSimpsons(float tStart, float tEnd, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //This is the resolution and has to be even
            int n = 20;
            //Now we need to divide the curve into sections
            float delta = (tEnd - tStart) / (float)n;
            //The main loop to calculate the length
            //Everything multiplied by 1
            float endPoints = GetArcLengthIntegrand(tStart, p0, p1, p2, p3) + GetArcLengthIntegrand(tEnd, p0, p1, p2, p3);
            //Everything multiplied by 4
            float x4 = 0f;
            for (int i = 1; i < n; i += 2)
            {
                float t = tStart + delta * i;
                x4 += GetArcLengthIntegrand(t, p0, p1, p2, p3);
            }
            //Everything multiplied by 2
            float x2 = 0f;
            for (int i = 2; i < n; i += 2)
            {
                float t = tStart + delta * i;
                x2 += GetArcLengthIntegrand(t, p0, p1, p2, p3);
            }
            //The final length
            float length = (delta / 3f) * (endPoints + 4f * x4 + 2f * x2);
            return length;
        }

        //Get the length of the curve with a naive method where we divide the
        //curve into straight lines and then measure the length of each line
        public static float GetLengthNaive(float tStart, float tEnd, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //This is the resolution, the higher the better
            int sections = 10;
            //Divide the curve into sections
            float delta = (tEnd - tStart) / (float)sections;
            //The start position of the curve
            Vector3 lastPos = GetCatmullRomPosition(tStart, p0, p1, p2, p3);
            //Init length
            float length = 0f;
            //Move along the curve
            for (int i = 1; i <= sections; i++)
            {
                //Calculate the t value at this section
                float t = tStart + delta * i;
                //Find the coordinates at this t
                Vector3 pos = GetCatmullRomPosition(t, p0, p1, p2, p3);
                //Add the section to the total length
                length += Vector3.Magnitude(pos - lastPos);
                //Save the latest pos for next loop
                lastPos = pos;
            }
            return length;
        }

        //Get and infinite small length from the derivative of the curve at position t
        private static float GetArcLengthIntegrand(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            //The derivative at this point (the velocity vector)
            Vector3 dPos = GetCatmullRomDerivative(t, p0, p1, p2, p3);
            //This the how it looks like in the YouTube videos
            //float xx = dPos.x * dPos.x;
            //float yy = dPos.y * dPos.y;
            //float zz = dPos.z * dPos.z;
            //float integrand = Mathf.Sqrt(xx + yy + zz);
            //Same as above
            float integrand = dPos.magnitude;
            return integrand;
        }

    }
}