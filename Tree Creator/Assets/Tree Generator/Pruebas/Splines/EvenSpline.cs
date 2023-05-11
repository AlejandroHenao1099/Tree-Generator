using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenSpline : MonoBehaviour
{
    [Range(0f, 1f)]
    public float tToSearch;
    public int resolutionCurve;
    public float sizeCube;
    public Transform[] points;
    // private List<float> lenghts;
    // private float lenghtSpline = 0f;
    //An array with colors to display the line segments that make up the final curve
    Color[] colorsArray = { Color.white, Color.red, Color.blue, Color.magenta, Color.black };

    // public EvenSpline(Vector3[] points)
    // {
    //     this.points = new List<Vector3>(points);
    // }

    public Vector3 GetPoint(float t)
    {
        float[] lenghts = new float[points.Length - 3];
        float lenghtSpline = 0f;
        float currentLenght = 0f;
        for (int i = 0; i < lenghts.Length; i++)
        {
            currentLenght = GetLengthSimpsons(0f, 1f, i, i + 1, i + 2, i + 3);
            lenghtSpline += currentLenght;
            lenghts[i] = currentLenght;
        }
        float valueOnCurve = Mathf.Lerp(0, lenghtSpline, t);
        int indexCurve = 0;
        currentLenght = 0;

        for (int i = 0; i < lenghts.Length; i++)
        {
            if (valueOnCurve >= currentLenght && valueOnCurve <= currentLenght + lenghts[i])
            {
                indexCurve = i;
                break;
            }
            currentLenght += lenghts[i];
        }

        float totallenghtCurve = lenghts[indexCurve];
        float d = Mathf.Abs(valueOnCurve - currentLenght);
        float tNewton = FindTValue(d, totallenghtCurve, indexCurve, indexCurve + 1, indexCurve + 2, indexCurve + 3);
        return GetCatmullRomPosition(tNewton, points[indexCurve].position,
            points[indexCurve + 1].position, points[indexCurve + 2].position, points[indexCurve + 3].position);
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

    //Use Newton–Raphsons method to find the t value at the end of this distance d
    float FindTValue(float d, float totalLength, int p0, int p1, int p2, int p3)
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

    float GetLengthSimpsons(float tStart, float tEnd, int p0, int p1, int p2, int p3)
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

    //Get and infinite small length from the derivative of the curve at position t
    float GetArcLengthIntegrand(float t, int p0, int p1, int p2, int p3)
    {
        //The derivative at this point (the velocity vector)
        Vector3 dPos = GetCatmullRomDerivative(t, points[p0].position, points[p1].position, points[p2].position, points[p3].position);
        //This the how it looks like in the YouTube videos
        //float xx = dPos.x * dPos.x;
        //float yy = dPos.y * dPos.y;
        //float zz = dPos.z * dPos.z;
        //float integrand = Mathf.Sqrt(xx + yy + zz);
        //Same as above
        float integrand = dPos.magnitude;
        return integrand;
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
        Vector3 lastPos = GetCatmullRomPosition(tStart, points[0].position, points[1].position, points[2].position, points[3].position);
        //Init length
        float length = 0f;
        //Move along the curve
        for (int i = 1; i <= sections; i++)
        {
            //Calculate the t value at this section
            float t = tStart + delta * i;
            //Find the coordinates at this t
            Vector3 pos = GetCatmullRomPosition(t, points[0].position, points[1].position, points[2].position, points[3].position);
            //Add the section to the total length
            length += Vector3.Magnitude(pos - lastPos);
            //Save the latest pos for next loop
            lastPos = pos;
        }
        return length;
    }

    private void OnDrawGizmos()
    {
        float step = 1f / resolutionCurve;
        Gizmos.color = Color.green;
        for (int i = 0; i <= resolutionCurve; i++)
        {
            var pos = GetPoint(i * step);
            Gizmos.DrawCube(pos, Vector3.one * sizeCube);
        }
        var posToSearch = GetPoint(tToSearch);
        Gizmos.color = Color.red;
        Gizmos.DrawCube(posToSearch, Vector3.one * sizeCube);

        // DivideCurveIntoSteps();
    }

    //Divide the curve into equal steps
    void DivideCurveIntoSteps()
    {
        //Find the total length of the curve
        float totalLength = GetLengthSimpsons(0f, 1f, 0, 1, 2, 3);
        // float totalLength = GetLengthNaive(0f, 1f);
        //How many sections do we want to divide the curve into
        int parts = 10;
        //What's the length of one section?
        float sectionLength = totalLength / (float)parts;
        //Init the variables we need in the loop
        float currentDistance = 0f + sectionLength;
        //The curve's start position
        // Vector3 lastPos = A;
        Vector3 lastPos = points[1].position;
        //The Bezier curve's color
        //Need a seed or the line will constantly change color
        Random.InitState(12345);
        int lastRandom = Random.Range(0, colorsArray.Length);
        for (int i = 1; i <= parts; i++)
        {
            //Use Newton–Raphsons method to find the t value from the start of the curve
            //to the end of the distance we have
            float t = FindTValue(currentDistance, totalLength, 0, 1, 2, 3);
            //Get the coordinate on the Bezier curve at this t value
            Vector3 pos = GetCatmullRomPosition(t, points[0].position, points[1].position, points[2].position, points[3].position);
            //Draw the line with a random color
            int newRandom = Random.Range(0, colorsArray.Length);
            //Get a different random number each time
            while (newRandom == lastRandom)
            {
                newRandom = Random.Range(0, colorsArray.Length);
            }
            lastRandom = newRandom;
            Gizmos.color = colorsArray[newRandom];
            Gizmos.DrawLine(lastPos, pos);
            //Save the last position
            lastPos = pos;
            //Add to the distance traveled on the line so far
            currentDistance += sectionLength;
        }
    }

}
