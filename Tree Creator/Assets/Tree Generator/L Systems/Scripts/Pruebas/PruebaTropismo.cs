using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PruebaTropismo : MonoBehaviour
{

    public Vector3 directionTropism;
    public float tropism;
    public float cantidadMovimiento;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculateTropism();
            var newPosition = transform.position + transform.forward * cantidadMovimiento;
            transform.position = newPosition;
        }
    }

    private void CalculateTropism()
    {
        var forward = transform.forward;
        var axis = Vector3.Cross(forward, directionTropism);
        var rateTropism = (tropism * axis.magnitude);
        var newForward = Vector3.Slerp(forward, directionTropism, rateTropism);
        var qTarget = Quaternion.LookRotation(directionTropism);
        var q = Quaternion.LookRotation(newForward);
        transform.rotation = Quaternion.Slerp(transform.rotation, qTarget, rateTropism);
    }
}