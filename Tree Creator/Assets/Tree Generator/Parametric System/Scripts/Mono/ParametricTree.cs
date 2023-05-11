using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Mathf;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ParametricTree : MonoBehaviour
{
    public int shape;
    public float generalRadius;
    public float radiusfactor = 0.7f;
    public int levels;
    public float baseSize;
    public float ratio = 0.015f;
    public float scale, scaleV;


    public LevelStemData level0, level1, level2;

    // private List<LevelStemData> steams = new List<LevelStemData>();
    private List<BranchData> branches = new List<BranchData>();

    // private List<Vector3> pointsSteam = new List<Vector3>();
    private List<float> radiusSteam = new List<float>();

    void Start()
    {
        CreateTrunk();
        CreateBranches1();
        CreateMesh();
    }

    private void CreateTrunk()
    {
        var currentBranch = new BranchData(0);
        currentBranch.pointsSteam.Add(Vector3.zero);
        currentBranch.orientation.SetTRS(Vector3.zero, Quaternion.Euler(-90f, 0f, 0f), Vector3.one);

        float scaleTree = scale + scaleV;
        float lenghtTrunk = (level0.nLength + level0.nLengthV) * scaleTree;
        level0.length_N = lenghtTrunk;

        float stepLenght = level0.length_N / (level0.nCurveRes);

        for (int i = 0; i < level0.nCurveRes; i++)
        {
            var currPos = currentBranch.orientation.MultiplyPoint3x4(Vector3.zero);
            var currentRotation = currentBranch.orientation.rotation;
            float angleRotation = 0;

            if (level0.nCurveBack == 0)
            {
                angleRotation = level0.nCurve / (float)level0.nCurveRes;
                var newRotation = Quaternion.Euler(angleRotation, 0, 0);
                currentBranch.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
            }
            else
            {
                if (i < Mathf.RoundToInt(level0.nCurveRes / 2))
                    angleRotation = level0.nCurve / (float)level0.nCurveRes / 2f;
                else
                    angleRotation = level0.nCurveBack / (float)level0.nCurveRes / 2f;

                angleRotation += (level0.nCurveV / (float)level0.nCurveRes) * RandomSign();

                var newRotation = Quaternion.Euler(angleRotation, 0, 0);
                currentBranch.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
            }

            var currDir = currentBranch.orientation.MultiplyVector(Vector3.forward);
            var newPos = currPos + currDir * stepLenght;
            currentBranch.pointsSteam.Add(newPos);
            currentBranch.orientation.SetTRS(newPos, currentBranch.orientation.rotation, Vector3.one);
        }
        // steams.Add(level0);
        branches.Add(currentBranch);
    }

    private void CreateBranches1()
    {
        var parentBranch = branches[0];
        var offset = 0f;
        float angleSplit = 0;

        for (int i = 1; i < parentBranch.pointsSteam.Count; i++)
        {
            #region ParentModification
            if (level0.nSegSplits > 0)
            {
                var prevBranch = new Branch(parentBranch.pointsSteam.ToArray(), Vector3.forward);

                float finalAngle = 0;
                if (angleSplit == 0)
                {
                    Vector3 currentDirSplit = (parentBranch.pointsSteam[i] - parentBranch.pointsSteam[i - 1]).normalized;
                    float declination = Mathf.Acos(currentDirSplit.y) * Rad2Deg;
                    angleSplit = level0.nSplitAngle + (level0.nSplitAngleV) - declination;
                    angleSplit = Mathf.Max(angleSplit, 0);
                    finalAngle = angleSplit;
                }
                else
                    finalAngle = -angleSplit / (float)parentBranch.pointsSteam.Count - 1;


                var newPrevPoints = prevBranch.Rotate(finalAngle, Axis.AxisX, i, true);
                for (int w = 0; w < parentBranch.pointsSteam.Count; w++)
                    parentBranch.pointsSteam[w] = newPrevPoints[w];
            }
            #endregion

            #region ChildCreation

            var branchChild = new BranchData(0);
            var currPos = parentBranch.pointsSteam[i];
            // var currDir = (parentBranch.pointsSteam[i + 1] - parentBranch.pointsSteam[i]).normalized;
            var currDir = (parentBranch.pointsSteam[i] - parentBranch.pointsSteam[i - 1]).normalized;
            offset += (parentBranch.pointsSteam[i - 1] - parentBranch.pointsSteam[i]).magnitude;
            var curRotation = Quaternion.LookRotation(currDir, Vector3.forward);

            float lenghtMax = level1.nLength + level1.nLengthV;

            var scaleTree = scale + scaleV;
            var lenghtBase = baseSize * scaleTree;
            var currRatio = (level0.length_N - offset) / (level0.length_N - lenghtBase);

            float shapeRatio = ShapeRatio(shape, currRatio);

            level1.length_N = level0.length_N * lenghtMax * shapeRatio;
            float stepLenght = level1.length_N / (level1.nCurveRes - 1);

            branchChild.pointsSteam.Add(currPos);


            var newRotation = Quaternion.identity;
            branchChild.orientation.SetTRS(currPos, curRotation, Vector3.one);

            // var q = curRotation;
            for (int j = 0; j < level1.nCurveRes; j++)
            {
                currPos = branchChild.orientation.MultiplyPoint3x4(Vector3.zero);
                var currentRotation = branchChild.orientation.rotation;
                float angleRotation = 0;

                if (Mathf.RoundToInt(level1.nCurveBack) == 0)
                {
                    angleRotation = level1.nCurve / (float)level1.nCurveRes;
                    newRotation = Quaternion.Euler(angleRotation, 0, 0);
                    // newRotation = Quaternion.AngleAxis(angleRotation, branchChild.orientation.MultiplyVector(Vector3.right));
                    branchChild.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
                }
                else
                {
                    if (i < Mathf.RoundToInt(level1.nCurveRes / 2))
                        angleRotation = level1.nCurve / (float)level1.nCurveRes / 2f;
                    else
                        angleRotation = level1.nCurveBack / (float)level1.nCurveRes / 2f;

                    angleRotation += ((level1.nCurveV / (float)level1.nCurveRes) * RandomSign());

                    newRotation = Quaternion.Euler(angleRotation, 0, 0);
                    branchChild.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
                }

                currDir = branchChild.orientation.MultiplyVector(Vector3.forward);
                var newPos = currPos + currDir * stepLenght;
                branchChild.pointsSteam.Add(newPos);
                branchChild.orientation.SetTRS(newPos, branchChild.orientation.rotation, Vector3.one);
                // branchChild.orientation.SetTRS(newPos, q, Vector3.one);
                // currPos = branchChild.orientation.MultiplyPoint3x4(Vector3.zero);
            }

            #endregion

            #region ClonesCreation            

            float stepAngle = 360f / (float)Mathf.RoundToInt(level0.nSegSplits);
            Vector3 up = (parentBranch.pointsSteam[i - 1] - parentBranch.pointsSteam[i]).normalized;
            var branch = new Branch(branchChild.pointsSteam.ToArray(), up);

            Vector3 angleRotationSplit = Vector3.up;
            float stepAngleSplitAxis = Mathf.RoundToInt(level0.nSegSplits) + 1;
            float downAngle = 0;

            if (level1.nDownAngleV >= 0)
                downAngle = level1.nDownAngle + level1.nDownAngleV;
            else
                downAngle = level1.nDownAngle + (level1.nDownAngleV * (1 - 2 * ShapeRatio(0, currRatio)));

            branch.Rotate(-downAngle, Axis.AxisX, 0, true);

            for (int n = 0; n < Mathf.RoundToInt(level0.nSegSplits); n++)
            {
                var newBranch = Clone(branchChild);
                var newPoints = branch.Rotate(stepAngle * n, Axis.AxisY);
                for (int k = 0; k < newBranch.pointsSteam.Count; k++)
                    newBranch.pointsSteam[k] = newPoints[k];

                branches.Add(newBranch);
            }

            #endregion
        }
    }

    private void CreateBranches2()
    {
        var parentBranch = branches[0];
        var offset = 0f;

        for (int i = 1; i < parentBranch.pointsSteam.Count - 1; i++)
        {
            var branchChild = new BranchData(0);
            var currPos = parentBranch.pointsSteam[i];
            var currDir = (parentBranch.pointsSteam[i + 1] - parentBranch.pointsSteam[i]).normalized;
            offset += (parentBranch.pointsSteam[i - 1] - parentBranch.pointsSteam[i]).magnitude;
            var curRotation = Quaternion.LookRotation(currDir);
            branchChild.pointsSteam.Clear();

            float lenghtMax = level1.nLength + (level1.nLengthV);


            var scaleTree = scale + scaleV;
            var lenghtBase = baseSize * scaleTree;
            var currRatio = (level0.length_N - offset) / (level0.length_N - lenghtBase);

            level1.length_N = level0.length_N * lenghtMax * ShapeRatio(shape, currRatio);
            float stepLenght = level1.length_N / (level1.nCurveRes - 1);


            branchChild.pointsSteam.Add(currPos);

            // float declination = Mathf.Acos(steamChild.orientation.MultiplyVector(Vector3.forward).z) * Mathf.Rad2Deg;
            // float splitAngle = steamChild.nSplitAngle + (steamChild.nSplitAngleV * RandomSign()) - declination;
            float downAngle = level1.nDownAngle + level1.nDownAngleV;
            // var newRotation = Quaternion.Euler(splitAngle, 0, 0);
            var newRotation = Quaternion.Euler(downAngle, 0, 0);
            branchChild.orientation.SetTRS(currPos, curRotation * newRotation, Vector3.one);


            for (int j = 0; j < level1.nCurveRes; j++)
            {
                currPos = branchChild.orientation.MultiplyPoint3x4(Vector3.zero);
                var currentRotation = branchChild.orientation.rotation;
                float angleRotation = 0;

                if (level1.nCurveBack == 0)
                {
                    angleRotation = level1.nCurve / (float)level1.nCurveRes;
                    newRotation = Quaternion.Euler(angleRotation, 0, 0);
                    branchChild.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
                }
                else
                {
                    if (i < Mathf.RoundToInt(level1.nCurveRes / 2))
                        angleRotation = level1.nCurve / (float)level1.nCurveRes / 2f;
                    else
                        angleRotation = level1.nCurveBack / (float)level1.nCurveRes / 2f;

                    angleRotation += (level1.nCurveV / (float)level1.nCurveRes) * RandomSign();

                    newRotation = Quaternion.Euler(angleRotation, 0, 0);
                    branchChild.orientation.SetTRS(currPos, currentRotation * newRotation, Vector3.one);
                }

                currDir = branchChild.orientation.MultiplyVector(Vector3.forward);
                var newPos = currPos + currDir * stepLenght;
                branchChild.pointsSteam.Add(newPos);
                branchChild.orientation.SetTRS(newPos, branchChild.orientation.rotation, Vector3.one);
            }
            branches.Add(branchChild);
        }
    }

    private BranchData Clone(BranchData branchData)
    {
        var returnBranch = new BranchData(0);
        for (int i = 0; i < branchData.pointsSteam.Count; i++)
            returnBranch.pointsSteam.Add(branchData.pointsSteam[i]);
        returnBranch.orientation = branchData.orientation;
        return returnBranch;
    }

    private void RotateBranch(ref BranchData branchData, Matrix4x4 orientation)
    {
        var factor = branchData.pointsSteam[0];
        for (int i = 0; i < branchData.pointsSteam.Count; i++)
            branchData.pointsSteam[i] -= factor;

        for (int i = 0; i < branchData.pointsSteam.Count; i++)
            branchData.pointsSteam[i] = orientation.MultiplyPoint3x4(branchData.pointsSteam[i]);

        for (int i = 0; i < branchData.pointsSteam.Count; i++)
            branchData.pointsSteam[i] += factor;
    }

    private float Length(LevelStemData stemData)
    {
        // float randomValueOne = Random.Range(0f, 1f) * 2f - 2f;
        // randomValueOne = Mathf.Sign(randomValueOne);
        // float randomValueTwo = Random.Range(0f, 1f) * 2f - 2f;
        // randomValueTwo = Mathf.Sign(randomValueTwo);
        return (scale + scaleV * RandomSign()) *
            (stemData.nLength + stemData.nLengthV * RandomSign());
    }

    private float RandomSign()
    {
        float randomValue = Random.Range(0f, 1f) * 2f - 2f;
        return Mathf.Sign(randomValue);
    }

    private float ShapeRatio(int shape, float ratio)
    {
        ratio = Mathf.Clamp01(ratio);
        switch (shape)
        {
            case 0:
                return 0.2f + 0.8f * ratio;
            case 1:
                return 0.2f + 0.8f * Sin(ratio * PI);
            case 2:
                return 0.2f + 0.8f * Sin(ratio * PI * 0.5f);
            case 3:
                return 1f;
            case 4:
                return 0.5f + 0.5f * ratio;
            case 5:
                return ratio <= 0.7f ? ratio / 0.7f : (1.0f - ratio) / 0.3f;
            case 6:
                return 1 - 0.8f * ratio;
            case 7:
                return ratio <= 0.7f ? 0.5f + 0.5f * ratio / 0.7f : 0.5f + 0.5f * (1 - ratio) / 0.3f;
            case 8:
                return 1;
            default:
                return 1;
        }
    }

    private void CreateMesh()
    {
        // Mesh mesh = new Mesh();
        // mesh.name = "Tree";
        // CombineInstance[] combineInstance = new CombineInstance[steams.Count];
        var currenRadius = generalRadius;
        for (int i = 0; i < branches.Count; i++)
        {
            var newContainer = new GameObject("Branch");
            newContainer.AddComponent<MeshFilter>().mesh = Points2Mesh.Create(branches[i].pointsSteam, null, 5, currenRadius);
            newContainer.AddComponent<MeshRenderer>().material = GetComponent<MeshRenderer>().material;
            newContainer.transform.SetParent(transform);
            if (i == 0)
                currenRadius = generalRadius * radiusfactor;
            // combineInstance[i] = new CombineInstance();
            // combineInstance[i].subMeshIndex = i;
            // combineInstance[i].mesh = Points2Mesh.Create(steams[i].pointsSteam, null, 5, generalRadius);
        }
        // mesh.CombineMeshes(combineInstance, true, false, false);
        // Mesh mesh = Points2Mesh.Create(trunk.pointsSteam, null, 5, generalRadius);
        // GetComponent<MeshFilter>().mesh = mesh;
    }

    // private void OnDrawGizmos()
    // {
    //     if (pointsSteam.Count > 0)
    //     {
    //         for (int i = 0; i < pointsSteam.Count; i++)
    //             Gizmos.DrawCube(pointsSteam[i], Vector3.one * sizeCube);
    //     }
    // }
}

[System.Serializable]
public struct LevelStemData
{
    // [HideInInspector]
    // public Matrix4x4 orientation;
    // public List<Vector3> pointsSteam;

    public float nLength, nLengthV;
    public float length_N;

    public float nCurveBack, nCurve, nCurveV;
    public int nCurveRes;


    public float nSegSplits, nBaseSplits, nSplitAngle, nSplitAngleV;
    public float nDownAngle, nDownAngleV;
}


[System.Serializable]
public struct BranchData
{
    [HideInInspector]
    public Matrix4x4 orientation;
    public List<Vector3> pointsSteam;

    public BranchData(int filler = 0)
    {
        pointsSteam = new List<Vector3>();
        orientation = new Matrix4x4();
    }
}