using System.Collections.Generic;
using UnityEngine;
using LindenmayerSystems;
using System.Diagnostics;

public class TreeGenerator : MonoBehaviour
{
    [TextArea]
    public string axiom;
    public int derivations = 0;
    public POLRules[] POLRules;
    public GlobalVariable[] globalVariables;
    public char[] ignore;
    public TurtleRules3D turtleRule;

    public int circleResolution;
    public Material materialBranch;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        // Stopwatch timer = new Stopwatch();
        // timer.Start();

        var pol = new POLSystem(axiom, derivations, POLRules, ignore, globalVariables);
        axiom = pol.CreateDP();

        var lindeTree = new TurtleParametric3D(axiom, turtleRule);
        var splinesData = lindeTree.Create();

        // var splines = Data2Spline(splinesData);
        // var i = 0;
        // foreach (var item in splines)
        // {
        //     var meshBranch = Spline2Mesh.Create(item, splinesData[i++].widths, circunferenceSplineResolution);
        //     var branch = new GameObject("Branch");
        //     branch.AddComponent<MeshFilter>().mesh = meshBranch;
        //     branch.AddComponent<MeshRenderer>().material = materialBranch;
        //     branch.transform.SetParent(transform);
        // }
        var treeMesh = new Tree2Mesh().Create(lindeTree.GetTree(), circleResolution);
        var tree = new GameObject("Tree");
        tree.AddComponent<MeshFilter>().mesh = treeMesh;
        tree.AddComponent<MeshRenderer>().material = materialBranch;
        tree.transform.SetParent(transform);
        // timer.Stop();
        // print(timer.ElapsedTicks);

        // 310 657 57 19
        // Olguita
    }

    private List<Splines> Data2Spline(List<SplineData> splineDatas)
    {
        var splines = new List<Splines>();
        for (int i = 0, len = splineDatas.Count; i < len; i++)
        {
            var currentSplineData = splineDatas[i];
            if (currentSplineData.points.Count <= 1)
                continue;
            var spl = new Splines(currentSplineData.points.ToArray(), false);
            splines.Add(spl);
        }
        return splines;
    }

    public enum TypeOLSystem
    {
        Deterministic,
        Stochastic,
        Parametric
    }
}