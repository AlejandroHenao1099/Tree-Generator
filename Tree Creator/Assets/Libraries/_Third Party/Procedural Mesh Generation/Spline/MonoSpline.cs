using System.Collections.Generic;
using UnityEngine;

public class MonoSpline : MonoBehaviour
{
    public ProceduralMeshGeneration.ShapeGenerator shapeGenerator;
    public Material materialCube;
    public float sizePoint;
    public int resolutionCurve;
    public bool addPointToSpline;
    [Min(0)]
    public int indexPointSelected;
    private int prevIndexSelected;

    public Vector3 positionPoint;

    private List<Transform> transPoints;

    public bool xAxis = false;


    // private MeshGenerator.DynamicSpline spline = new MeshGenerator.DynamicSpline(
    //     new Vector3[] { -Vector3.forward, Vector3.zero, Vector3.forward, Vector3.forward * 2 }
    // );
    private MeshGenerator.DynamicSpline spline;

    private void Awake()
    {
        transPoints = new List<Transform>();
        var newPoints = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            var newPoint = new GameObject("Point");
            // var newPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
            newPoint.AddComponent<MeshFilter>().mesh = MeshGenerator.Cube.Create(2, 2, 2, sizePoint, sizePoint, sizePoint);
            newPoint.AddComponent<MeshRenderer>().material = materialCube;
            // newPoint.AddComponent<DoodleSelection>();
            // newPoint.AddComponent<Rigidbody>().isKinematic = true;
            // newPoint.AddComponent<SphereCollider>().radius = 0.1f;
            // newPoint.GetComponent<SphereCollider>().isTrigger = true;
            transPoints.Add(newPoint.transform);
            transPoints[i].SetParent(transform);
            transPoints[i].localScale = Vector3.one;
            var startPos = xAxis == true ? -Vector3.right + Vector3.right * i :
                -Vector3.forward + Vector3.forward * i; ;
            transPoints[i].localPosition = newPoints[i] = startPos;
        }
        spline = new MeshGenerator.DynamicSpline(newPoints);
        positionPoint = transPoints[0].localPosition;
    }

    public Vector3 GetPoint(float t)
    {
        return spline.GetPoint(t);
    }

    private void OnValidate()
    {
        if (addPointToSpline == true)
        {
            AddCurvePoint();
            addPointToSpline = false;
        }
        if (indexPointSelected >= spline.QuantityPoints || indexPointSelected < 0)
            indexPointSelected = 0;

        if (indexPointSelected != prevIndexSelected)
        {
            positionPoint = transPoints[indexPointSelected].localPosition;
            prevIndexSelected = indexPointSelected;
        }
        transPoints[indexPointSelected].localPosition = positionPoint;
    }

    public void AddCurvePoint()
    {
        var newPoint = new GameObject("Point");
        // var newPoint = GameObject.CreatePrimitive(PrimitiveType.Cube);
        newPoint.AddComponent<MeshFilter>().mesh = MeshGenerator.Cube.Create(2, 2, 2, sizePoint, sizePoint, sizePoint);
        newPoint.AddComponent<MeshRenderer>().material = materialCube;
        transPoints.Add(newPoint.transform);
        var i = transPoints.Count - 1;
        transPoints[i].SetParent(transform);
        transPoints[i].localScale = Vector3.one;
        var newPointPos = spline[spline.QuantityPoints - 1] +
            (spline[spline.QuantityPoints - 1] - spline[spline.QuantityPoints - 2]);
        transPoints[i].localPosition = newPointPos;
        spline.AddPoint(newPointPos);
    }

    private void UpdateSpline()
    {
        for (int i = 0; i < transPoints.Count; i++)
            spline.UpdatePoint(transPoints[i].localPosition, i);
        spline.UpdateSpline();
        shapeGenerator.UpdatePlane();
    }

    private void OnDrawGizmos()
    {
        UpdateSpline();
        // Gizmos.color = Color.red;
        // for (int i = 0; i < transPoints.Count; i++)
        //     Gizmos.DrawCube(transPoints[i].localPosition, Vector3.one * sizePoint);

        Gizmos.color = Color.green;
        Gizmos.DrawLine(transPoints[0].localPosition, transPoints[1].localPosition);
        Gizmos.DrawLine(transPoints[transPoints.Count - 2].localPosition, transPoints[transPoints.Count - 1].localPosition);

        Gizmos.color = Color.yellow;
        float stepCurve = 1f / (resolutionCurve - 1);
        for (int i = 0; i < resolutionCurve - 1; i++)
        {
            var start = spline.GetPoint(stepCurve * i);
            var end = spline.GetPoint(stepCurve * (i + 1));
            Gizmos.DrawLine(start, end);
        }
    }


}
