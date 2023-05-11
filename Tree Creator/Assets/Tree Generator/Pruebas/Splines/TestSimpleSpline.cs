using System.Collections.Generic;
using UnityEngine;

public class TestSimpleSpline : MonoBehaviour
{

    public float sizeRay = 1f;
    public int resolutionCurve;
    public float sizeCube;


    [Range(0f, 1f)]
    public float tToSearch;

    public bool loop;
    public bool incrementResolution;
    public int quantityIncrementResolution;

    public bool deletePoint;
    public int indexToDelete;

    public bool updateSegment;
    public int segmentToUpdate;
    public StateSegment stateSegment;

    public bool addPoint;

    public List<Transform> points;
    public Transform newPoint;

    public MeshGenerator.DynamicSpline dynamicSpline;
    public MeshGenerator.DynamicLine line;
    [SerializeField]
    public MeshGenerator.DynamicHybridSpline hybrid;

    public TypeLine state;

    bool onStart;

    private void Start()
    {
        var vecPoints = new Vector3[points.Count];
        for (int i = 0; i < vecPoints.Length; i++)
            vecPoints[i] = points[i].position;
        dynamicSpline = new MeshGenerator.DynamicSpline(vecPoints);
        line = new MeshGenerator.DynamicLine(vecPoints);
        hybrid = new MeshGenerator.DynamicHybridSpline(vecPoints);
        onStart = true;
    }

    private void OnValidate()
    {
        if (incrementResolution)
        {
            IncreaseResolutionSpline();
            incrementResolution = false;
        }
        else if (deletePoint)
        {
            DeletePoint();
            deletePoint = false;
        }
        else if (addPoint)
        {
            AddPoint();
            addPoint = false;
        }
        // else if (loop != dynamicSpline.Loop)
        else if (loop != line.Loop)
        {
            dynamicSpline.Loop = loop;
            line.Loop = loop;
            hybrid.Loop = loop;
        }
        else if(updateSegment)
        {
            hybrid.UpdateStateSegment(segmentToUpdate, stateSegment);
            updateSegment = false;
        }
    }

    private void OnDrawGizmos()
    {
        if (!onStart) return;
        UpdatePoints();

        Gizmos.color = Color.green;
        float step = 1f / resolutionCurve;
        for (int i = 0; i < resolutionCurve; i++)
        {
            var start = Vector3.zero;
            var end = Vector3.zero;
            if (state == TypeLine.Curve)
            {
                start = dynamicSpline.GetPoint(step * i);
                end = dynamicSpline.GetPoint(step * (float)(i + 1));

            }
            else if (state == TypeLine.Hybrid)
            {
                start = hybrid.GetPoint(step * i);
                end = hybrid.GetPoint(step * (float)(i + 1));
            }
            else
            {
                start = line.GetPoint(step * i);
                end = line.GetPoint(step * (float)(i + 1));
            }
            Gizmos.DrawLine(start, end);
        }

        // Gizmos.color = Color.red;

        // step = 1f / (resolutionCurve - 1);
        // for (int i = 0; i < resolutionCurve; i++)
        // {
        //     var start = Vector3.zero;
        //     var end = Vector3.zero;
        //     if (state == TypeLine.Curve)
        //     {
        //         start = dynamicSpline.GetPoint(step * i);
        //         end = dynamicSpline.GetDerivative(step * i);
        //     }
        //     else if (state == TypeLine.Hybrid)
        //     {
        //         start = hybrid.GetPoint(step * i);
        //         end = hybrid.GetDerivative(step * i);
        //     }
        //     else
        //     {
        //         start = line.GetPoint(step * i);
        //         end = line.GetDerivative(step * i);
        //     }
        //     Gizmos.DrawRay(start, end * sizeRay);
        // }

        // Gizmos.color = Color.yellow;
        // for (int i = 0; i < resolutionCurve; i++)
        // {
        //     var start = dynamicSpline.GetPoint(step * i);
        //     var end = dynamicSpline.GetNormal(step * i, Vector3.up);
        //     Gizmos.DrawRay(start, end * sizeRay);
        //     // Gizmos.DrawLine(start, start + end);
        // }



        var posToSearch = Vector3.zero;
        if (state == TypeLine.Curve)
            posToSearch = dynamicSpline.GetPoint(tToSearch);
        else if (state == TypeLine.Hybrid)
            posToSearch = hybrid.GetPoint(tToSearch);
        else
            posToSearch = line.GetPoint(tToSearch);
        Gizmos.color = Color.cyan;
        Gizmos.DrawCube(posToSearch, Vector3.one * sizeCube);
    }

    private void UpdatePoints()
    {
        var vecPoints = new Vector3[points.Count];
        for (int i = 0; i < vecPoints.Length; i++)
        {
            var point = points[i].position;
            if (point != line[i])
            {
                line.UpdatePoint(point, i, true);
                dynamicSpline.UpdatePoint(point, i, true);
                hybrid.UpdatePoint(point, i, true);
            }
            // if (point != dynamicSpline[i])
        }
    }

    private void DeletePoint()
    {
        dynamicSpline.RemovePoint(indexToDelete, true);
        line.RemovePoint(indexToDelete, true);
        hybrid.RemovePoint(indexToDelete, true);
        var point = points[indexToDelete].gameObject;
        points.RemoveAt(indexToDelete);
        Destroy(point);
    }

    private void IncreaseResolutionSpline()
    {
        dynamicSpline.IncreaseResolution(quantityIncrementResolution);
        line.IncreaseResolution(quantityIncrementResolution);
        hybrid.IncreaseResolution(quantityIncrementResolution);
        for (int i = 0; i < quantityIncrementResolution; i++)
        {
            var point = Instantiate(newPoint.gameObject);
            points.Add(point.transform);
        }

        for (int i = 0; i < points.Count; i++)
            points[i].position = line[i];
    }

    private void AddPoint()
    {
        var point = Instantiate(newPoint.gameObject);
        dynamicSpline.AddPoint(point.transform.position, true);
        line.AddPoint(point.transform.position, true);
        hybrid.AddPoint(point.transform.position, StateSegment.Curve, true);
        points.Add(point.transform);
    }


}

public enum TypeLine
{
    Curve, Line, Hybrid
}
