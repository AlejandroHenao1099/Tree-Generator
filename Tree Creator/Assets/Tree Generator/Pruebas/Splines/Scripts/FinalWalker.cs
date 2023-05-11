using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalWalker : MonoBehaviour
{
    public Transform target;
    public float lenghtAxes;
    public bool updateAxis;

    private void OnDrawGizmos()
    {
        if (updateAxis == true)
        {
            var newDir = (target.position - transform.position).normalized;
            var currentUp = transform.up;
            var angle = Mathf.Acos(Vector3.Dot(newDir, currentUp)) * Mathf.Rad2Deg;
            var normal = Vector3.Cross(newDir, currentUp).normalized;
            var q = Quaternion.LookRotation(newDir, normal);
            var newUp = (q * Quaternion.Euler(0, 90, 0)) * Vector3.forward;
            if (Vector3.Dot(currentUp, newUp) > Vector3.Dot(currentUp, -newUp))
            {
                var newRot = Quaternion.LookRotation(newDir, newUp);
                transform.rotation = newRot;
            }
            else
            {
                var newRot = Quaternion.LookRotation(newDir, -newUp);
                transform.rotation = newRot;
            }
            updateAxis = false;
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, transform.forward * lenghtAxes);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, transform.right * lenghtAxes);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Vector3.zero, transform.up * lenghtAxes);
    }
}
