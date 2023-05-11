using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestFunction : MonoBehaviour
{

    public float taper = 0;
    public float ratio;
    public float lenghtTrunk;
    public float nScale;

    public int quantityPoints;
    public float sizeCube;
    public Color colorCube;
    public Color colorRay;

    bool onStart;
    Vector3[] positions;

    private void Start()
    {
        positions = new Vector3[quantityPoints + 1];

        onStart = true;

        float step = 1f / quantityPoints;
        for (int i = 0; i <= quantityPoints; i++)
        {

            var currPos = new Vector3(step * i, GetRadius(step * i), 0);
        }

    }


    private void OnDrawGizmos()
    {
        // if (onStart == false) return;
        // Gizmos.color = colorCube;
        // float step = 1f / (quantityPoints + 1);
        // for (int i = 0; i <= quantityPoints; i++)
        //     positions[i] = new Vector3(step * i, GetRadius(step * i), 0);

        // for (int i = 0; i < quantityPoints; i++)
        // {
        //     Gizmos.DrawRay(positions[i],positions[i + 1]);
        // }
        float step = 1f / quantityPoints;
        var prevPos = Vector3.zero;
        for (int i = 0; i <= quantityPoints; i++)
        {
            var currPos = new Vector3(step * i, GetRadius(step * i), 0);
            Gizmos.color = colorCube;
            Gizmos.DrawCube(currPos, Vector3.one * sizeCube);
            Gizmos.color = colorRay;
            Gizmos.DrawLine(prevPos, currPos);
            prevPos = currPos;
        }
    }

    private float GetRadius(float z)
    {
        z = Mathf.Clamp01(z); // z = 0.95
        float unit_taper = GetUnitTaper(); // 0
        float radiusStem = lenghtTrunk * ratio * nScale; // = 0.2128
        float taperZ = radiusStem * (1f - unit_taper * z); // = 0.2128

        float radiusZ = 0f;
        if (taper >= 0f && taper < 1f)
            radiusZ = taperZ;
        else if (taper >= 1f && taper <= 3f)
        {
            float z_2 = (1f - z) * lenghtTrunk; // = 0.2
            float depth = 0f;
            if (taper < 2f || z_2 < taperZ)
                depth = 1f; // = 1
            else
                depth = taper - 2f;

            float z_3 = 0f;
            if (taper < 2f)
                z_3 = z_2;
            else // int = 1 o puede ser 0
                // z_3 = z_2 - 2f * taperZ * Mathf.RoundToInt(z_2 / (2f * taperZ) + 0.5f); // = -0.2256
                z_3 = z_2 - 2f * taperZ * (int)(z_2 / (2f * taperZ) + 0.5f); // = 0.2  => int = 0

            if (taper < 2f && z_3 >= taperZ)
                radiusZ = taperZ;
            else
                radiusZ = (1f - depth) * taperZ + depth *
                    Mathf.Sqrt(Mathf.Abs((taperZ * taperZ) - (z_3 - taperZ) * (z_3 - taperZ)));
                // = (1 - 1) * 0.2128 + 1 * sqrt(0.2128^2 - (-0.2256 - 0.2128)^2 )  
                // 0.2128^2 = 0.04528384
                // (-0.2256 - 0.2128)^2 = 0.19219456
        }
        // Debug.Log("Z: " + z + " Radius: " + radiusZ);
        // Debug.Log(z);
        // Debug.Log(radiusZ);
        return radiusZ;
    }

    private float GetUnitTaper()
    {
        if (taper >= 0f && taper < 1f)
            return taper;
        if (taper >= 1f && taper < 2f)
            return 2f - taper;
        // if (trunkData.nTaper >= 2f && trunkData.nTaper < 3f)
        return 0f;
    }



}
