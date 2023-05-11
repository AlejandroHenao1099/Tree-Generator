using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementReview : MonoBehaviour
{
    public float speed;
    public float angularSpeed;


    void Update()
    {
        Vector3 velocity = transform.forward * Input.GetAxis("Vertical") * speed * Time.deltaTime;
        transform.position += velocity;
        Vector3 angularVelocity = new Vector3(0, Input.GetAxis("Horizontal") * angularSpeed * Time.deltaTime, 0);
        transform.rotation *= Quaternion.Euler(angularVelocity);
        if (Input.GetKey(KeyCode.O))
        {
            angularVelocity = new Vector3(angularSpeed * Time.deltaTime, 0, 0);
            transform.rotation *= Quaternion.Euler(angularVelocity);
        }
        else if (Input.GetKey(KeyCode.P))
        {
            angularVelocity = new Vector3(-angularSpeed * Time.deltaTime, 0, 0);
            transform.rotation *= Quaternion.Euler(angularVelocity);
        }
    }
}
