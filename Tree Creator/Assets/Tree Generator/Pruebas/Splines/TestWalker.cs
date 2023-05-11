using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MeshGenerator;

public class TestWalker : MonoBehaviour
{
    public Transform target;
    WalkerSpline walkerSpline;
    public float sizeCube, lenghtAxes, lenghtPlanes, lenghMinus, lenghtUp;

    public float upLimit, rightLimit, limitProject;

    private Matrix4x4 matrix = new Matrix4x4();
    public bool onStart = false;
    private Stack<Vector3> stackUp;
    public int quantityStack = 5;
    public bool heightSet;
    float heightProject = 0f;
    public float errorToUpdate = 0.05f;

    private List<Vector3> path;
    private List<Quaternion> orientations;
    Vector3 forward, right, left, up, down;
    Vector3 minusRight, minusLeft;
    Vector3 prevPos;

    private void Start()
    {
        path = new List<Vector3>();
        orientations = new List<Quaternion>();
        walkerSpline = new WalkerSpline(Vector3.forward, Vector3.up);
        path.Add(Vector3.zero);
        orientations.Add(Quaternion.LookRotation(Vector3.forward, Vector3.up));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            var currPos = walkerSpline.GetPoint(Vector3.zero);
            var newDir = (target.position - currPos).normalized;
            walkerSpline.UpdatePosition(target.position);
            walkerSpline.UpdateDirection(newDir);
            path.Add(target.position);
            var forw = walkerSpline.GetPoint(Vector3.forward);
            var up = walkerSpline.GetPoint(Vector3.up);
            var q = Quaternion.LookRotation(forw, up);
            orientations.Add(q);
        }
    }

    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.magenta;
        // Gizmos.DrawRay(Vector3.zero, (target.position - transform.position).normalized);
        // Gizmos.color = Color.blue;
        // Gizmos.DrawRay(Vector3.zero, transform.forward);
        // Gizmos.color = Color.red;
        // Gizmos.DrawRay(Vector3.zero, transform.right);
        // Gizmos.color = Color.green;
        // Gizmos.DrawRay(Vector3.zero, transform.up);
        UpdateAxis();

        if (path == null) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < path.Count; i++)
            Gizmos.DrawCube(path[i], Vector3.one * sizeCube);

        for (int i = 0; i < orientations.Count; i++)
        {
            Gizmos.color = Color.blue;
            var dir = orientations[i] * Vector3.forward;
            Gizmos.DrawRay(path[i], dir * lenghtAxes);

            Gizmos.color = Color.green;
            dir = orientations[i] * Vector3.up;
            Gizmos.DrawRay(path[i], dir * lenghtAxes);

            Gizmos.color = Color.red;
            dir = orientations[i] * Vector3.right;
            Gizmos.DrawRay(path[i], dir * lenghtAxes);
        }
    }

    private void UpdateAxis()
    {
        if (onStart == false)
        {
            matrix.SetTRS(Vector3.zero, Quaternion.identity, Vector3.one);
            stackUp = new Stack<Vector3>();
            for (int i = 0; i < quantityStack; i++)
                stackUp.Push(Vector3.up);

            onStart = true;
        }
        if (prevPos.Equals(target.position))
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.forward) * lenghtAxes);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.right) * lenghtAxes);
            Gizmos.color = Color.green;
            Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.up) * lenghtUp);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(Vector3.zero, up * lenghtPlanes);
            Gizmos.color = Color.gray;
            Gizmos.DrawRay(Vector3.zero, down * lenghtPlanes);
            Gizmos.color = Color.magenta;
            Gizmos.DrawRay(Vector3.zero, right * lenghtPlanes);
            Gizmos.color = Color.cyan;
            Gizmos.DrawRay(Vector3.zero, left * lenghtPlanes);

            Gizmos.color = Color.black;
            Gizmos.DrawRay(transform.position, minusRight * lenghMinus);
            Gizmos.color = Color.white;
            Gizmos.DrawRay(transform.position, minusLeft * lenghMinus);
            // heightSet = false;
            return;
        }


        prevPos = target.position;
        var prevDir = matrix.MultiplyVector(Vector3.forward);
        var prevUp = matrix.MultiplyVector(Vector3.up);
        forward = (target.position - transform.position).normalized;
        right = (prevDir - forward).normalized;

        up = Vector3.Cross(forward, right).normalized;
        right = Vector3.Cross(forward, up);

        down = -up;
        left = -right;
        var newUp = Vector3.zero;

        // var projectUp = Vector3.Project(prevUp, up);

        var nearUp = Vector3.Dot(prevUp, up) > Vector3.Dot(prevUp, down) ? up : down;
        // var heightProject = Vector3.Project(prevUp, nearUp).magnitude;
        // if (heightSet == false)
        // {
        heightProject = Vector3.Dot(nearUp, prevUp);
        //     heightSet = true;
        //     print(-3);
        // }
        GetMinusVectors(heightProject, nearUp, out minusRight, out minusLeft);
        var nearDir = NearestParallelVector(prevUp, nearUp, right, left, minusLeft, minusRight);

        newUp = nearDir;

        // var cosUp = Vector3.Dot(prevUp, nearUp);
        // var sinUp = Mathf.Sqrt(1f - (cosUp * cosUp));
        // var minusUpBase = nearUp * heightProject;
        // minusRight = (minusUpBase + (right * sinUp)).normalized;
        // minusLeft = (minusUpBase + (left * sinUp)).normalized;

        // var minusLateral = Vector3.Dot(prevUp, minusRight) > Vector3.Dot(prevUp, minusLeft) ? minusRight : minusLeft;

        // var dotTransUp = Vector3.Dot(up, prevUp);
        // var nearRight = Vector3.zero;

        // if (dotTransUp >= upLimit || dotTransUp <= -upLimit)
        // {
        //     // print(0);
        //     if (Vector3.Dot(prevUp, up) > Vector3.Dot(prevUp, down))
        //         // newUp = up;
        //         nearUp = up;
        //     else
        //         // nearUp = down;
        //         newUp = down;
        // }
        // else if (dotTransUp <= rightLimit && dotTransUp >= -rightLimit)
        // {
        //     // print(1);
        //     if (Vector3.Dot(prevUp, right) > Vector3.Dot(prevUp, left))
        //         newUp = right;
        //     // nearRight = right;
        //     else
        //         // nearRight = left;
        //         newUp = left;
        // }
        // else
        // {
        //     // print(-1);
        //     // var nearUp = Vector3.Dot(prevUp, up) > 0 ? up : down;
        //     // var cosUp = Vector3.Dot(prevUp, nearUp);
        //     // var sinUp = Mathf.Sqrt(1f - (cosUp * cosUp));
        //     // var heightProject = Vector3.Project(prevUp, nearUp).magnitude;
        //     // var minusUpBase = nearUp * heightProject;
        //     // minusRight = (minusUpBase + (right * sinUp)).normalized;
        //     // minusLeft = (minusUpBase + (left * sinUp)).normalized;

        //     // var minusLateral = (prevUp - minusRight).magnitude <
        //     //     (prevUp - minusLeft).magnitude ? minusRight : minusLeft;
        //     // float dotMinus = Vector3.Dot(prevUp, minusLateral);
        //     // float dotLeft = Vector3.Dot(prevUp, left);
        //     // float dotRight = Vector3.Dot(prevUp, right);
        //     // float dotUp = Vector3.Dot(prevUp, up);
        //     // float dotDown = Vector3.Dot(prevUp, down);

        //     // if (dotMinus > dotRight && dotMinus > dotLeft && dotMinus > dotUp && dotMinus > dotDown)

        //     newUp = minusLateral;
        //     // else if (dotLeft > dotRight && dotLeft > dotUp && dotLeft > dotDown)
        //     //     newUp = left;
        //     // else if (dotRight > dotLeft && dotRight > dotUp && dotRight > dotDown)
        //     //     newUp = right;
        //     // else if (dotUp > dotLeft && dotUp > dotRight && dotUp > dotDown)
        //     //     newUp = up;
        //     // else
        //     //     newUp = down;
        // }

        // float dotLateral = Vector3.Dot(prevUp, minusLateral);
        // float dotFUp = Vector3.Dot(prevUp, nearUp);
        // float dotFRight = Vector3.Dot(prevUp, nearRight);

        // var nearest = dotLateral > dotFUp && dotLateral > dotFRight ? minusLateral :
        //     dotFUp > dotFRight ? nearUp : nearRight;

        // if(Vector3.Dot(nearest, prevUp) > (1f - errorToUpdate))
        // {
        //     newUp = nearest;
        //     print(0);
        // }
        // else
        // {
        //     print(-1);
        //     newUp = prevUp;
        // }

        // stackUp.Pop();
        // stackUp.Push(newUp);
        // newUp = Vector3.zero;
        // foreach (var upAxis in stackUp)
        //     newUp += upAxis;
        // // newUp /= stackUp.Count;
        // newUp = newUp.normalized;

        var q = Quaternion.LookRotation(forward, newUp);
        var finalRot = Quaternion.Slerp(matrix.rotation, q, 1f);
        matrix.SetTRS(Vector3.zero, finalRot, Vector3.one);


        Gizmos.color = Color.blue;
        Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.forward) * lenghtAxes);
        Gizmos.color = Color.red;
        Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.right) * lenghtAxes);
        Gizmos.color = Color.green;
        Gizmos.DrawRay(Vector3.zero, matrix.MultiplyVector(Vector3.up) * lenghtUp);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Vector3.zero, up * lenghtPlanes);
        Gizmos.color = Color.gray;
        Gizmos.DrawRay(Vector3.zero, down * lenghtPlanes);
        Gizmos.color = Color.magenta;
        Gizmos.DrawRay(Vector3.zero, right * lenghtPlanes);
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(Vector3.zero, left * lenghtPlanes);

        Gizmos.color = Color.black;
        Gizmos.DrawRay(transform.position, minusRight * lenghMinus);
        Gizmos.color = Color.white;
        Gizmos.DrawRay(transform.position, minusLeft * lenghMinus);

        // transform.LookAt(newDir);
    }

    private Vector3 NearestParallelVector(Vector3 target, params Vector3[] directions)
    {
        int indexNearest = -1;
        float maxDot = float.MinValue;
        for (int i = 0; i < directions.Length; i++)
        {
            var currDot = Vector3.Dot(target, directions[i]);
            if (currDot > maxDot)
            {
                indexNearest = i;
                maxDot = currDot;
            }
        }
        return directions[indexNearest];
    }

    private void GetMinusVectors(float height, Vector3 upMinus, out Vector3 minusRight, out Vector3 minusLeft)
    {
        var sinUp = 0f;
        var heightUpMinus = 0f;
        if (height >= 0.98480775301220805936674302458952f) // 0 grados
        {
            print(0);
            minusRight = upMinus.normalized;
            minusLeft = upMinus.normalized;
            return;
        }
        else if (height <= 0.17364817766693034885171662676931f) //90 grados
        {
            print(90);
            minusRight = right;
            minusLeft = left;
            return;
        }
        else if (height > 0.93969262078590838405410927732473f && height < 0.98480775301220805936674302458952f) // 15 grados
        {
            print(15);
            heightUpMinus = 0.9659258262890682867497431997289f;
            sinUp = 0.25881904510252076234889883762405f;
        }
        else if (height > 0.8f && height <= 0.93969262078590838405410927732473f) //30 grados
        {
            print(30);
            heightUpMinus = 0.86602540378443864676372317075294f;
            sinUp = 0.5f;
        }
        else if (height <= 0.8f && height > 0.6f) //45 grados
        {
            print(45);
            heightUpMinus = 0.70710678118654752440084436210485f;
            sinUp = 0.70710678118654752440084436210485f;
        }
        else if (height <= 0.6f && height > 0.4f) //60 grados
        {
            print(60);
            heightUpMinus = 0.5f;
            sinUp = 0.86602540378443864676372317075294f;
        }
        else if (height <= 0.4f && height > 0.17364817766693034885171662676931f) //75 grados
        {
            print(75);
            heightUpMinus = 0.25881904510252076234889883762405f;
            sinUp = 0.9659258262890682867497431997289f;
        }
        else
        {
            print(-1);
            heightUpMinus = 0f;
            sinUp = 1f;
        }

        // var cosUp = Vector3.Dot(prevUp, upMinus);
        // sinUp = Mathf.Sqrt(1f - (cosUp * cosUp));
        var minusUpBase = upMinus * heightUpMinus;
        minusRight = (minusUpBase + (right * sinUp)).normalized;
        minusLeft = (minusUpBase + (left * sinUp)).normalized;
    }


}
