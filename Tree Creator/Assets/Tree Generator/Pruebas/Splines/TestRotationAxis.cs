using System.Collections.Generic;
using UnityEngine;

public class TestRotationAxis : MonoBehaviour
{
    public Transform target;
    public float angleRotation;

    public bool updateRotation, restartRotation;

    void Update()
    {
        if (updateRotation)
        {
            var newDir = (target.position - transform.position).normalized;
            var newUp = Vector3.Cross(Vector3.forward, newDir).normalized;
            newUp = Vector3.Dot(newUp, Vector3.up) > 0 ? newUp : -newUp;
            var anglebetween = Vector3.Angle(newDir, Vector3.forward);
            var q = Quaternion.AngleAxis(anglebetween, newUp);
            transform.rotation = q;
            updateRotation = false;
        }
        else if (restartRotation)
        {
            transform.rotation = Quaternion.identity;
            restartRotation = false;

        }
    }


}
