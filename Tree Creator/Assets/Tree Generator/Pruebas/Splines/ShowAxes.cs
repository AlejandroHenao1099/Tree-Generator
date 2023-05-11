using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowAxes : MonoBehaviour
{
    public float lenghtAxes;

    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, transform.forward * lenghtAxes);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, transform.right * lenghtAxes);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Vector3.zero, transform.up * lenghtAxes);
    }


    // el eje Up se dirige hacia la nueva direccion, pero manteniendo su ortogonalidad
    // se puede calcular esto

}
