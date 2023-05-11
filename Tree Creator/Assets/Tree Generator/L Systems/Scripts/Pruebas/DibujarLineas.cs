using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DibujarLineas : MonoBehaviour
{
    public Vector3 heading = new Vector3(0, 0, 1);
    public float angleRotation = 30;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            heading = transform.forward;
            transform.forward = Quaternion.Euler(0, angleRotation, 0) * heading;
        }
    }
}
