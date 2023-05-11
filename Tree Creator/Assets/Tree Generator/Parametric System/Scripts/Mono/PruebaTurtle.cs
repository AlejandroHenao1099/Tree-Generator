using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TreeCreator;

public class PruebaTurtle : MonoBehaviour
{
    public float angle;
    public float distanceMovement;
    public bool rotate;
    public Vector3 biasDirection;
    public float magnitudeBias;
    Turtle turtle;


    void Start()
    {
        turtle = new Turtle(0);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            turtle.Turn(-angle);
        else if (Input.GetKeyDown(KeyCode.D))
            turtle.Turn(angle);
        else if (Input.GetKeyDown(KeyCode.W))
            turtle.Pitch(-angle);
        else if (Input.GetKeyDown(KeyCode.S))
            turtle.Pitch(angle);
        else if (Input.GetKeyDown(KeyCode.Q))
            turtle.Roll(angle);
        else if (Input.GetKeyDown(KeyCode.E))
            turtle.Roll(-angle);
        else if (Input.GetKeyDown(KeyCode.B))
            turtle.Bias(biasDirection.normalized, magnitudeBias);
        else if (Input.GetKeyDown(KeyCode.Space))
            turtle.Move(distanceMovement);

        
    }

    private void OnDrawGizmos()
    {
        var forward = turtle.GetForward();
        var right = turtle.GetRight();
        var up = turtle.GetUp();
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(turtle.GetPosition(), forward * 2f);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(turtle.GetPosition(), right * 2f);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(turtle.GetPosition(), up * 2f);
    }
}
