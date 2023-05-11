using System.Diagnostics;
using System.Collections.Generic;
using UnityEngine;
using LindenmayerSystems;

public class Lindenmayer3DEditor : MonoBehaviour
{
    [TextArea]
    public string axiom;
    public int derivations = 0;
    public DOLRules[] DOLRules;
    public SOLRules[] SOLRules;
    public char[] ignore;
    public TurtleRules3D turtleRule;
    public Color color;
    public GameObject prefab;

    private List<Vector3> points;
    private List<Vector3> leavesPoints;

    public bool stochastic;


    void Start()
    {
        Initialize();
        PutLeaves();
    }

    private void Initialize()
    {
        if (stochastic)
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var sol = new SOLSystem(axiom, derivations, SOLRules, ignore);
            axiom = sol.Create();
            timer.Stop();
            print(timer.ElapsedTicks);
        }
        else
        {
            var dol = new DOLSystem(axiom, derivations, DOLRules, ignore);
            axiom = dol.Create();
        }

        leavesPoints = new List<Vector3>();
        var lin3D = new Lindenmayer3D(axiom, turtleRule);
        points = lin3D.Create(leavesPoints);

        // 310 657 57 19
        // Olguita
    }

    private void PutLeaves()
    {
        for (int i = 0, len = leavesPoints.Count; i < len; i++)
        {
            var pref = Instantiate(prefab, points[i], Quaternion.identity);
            pref.transform.SetParent(transform);
        }
    }

    private void OnDrawGizmos()
    {
        if (points != null)
        {
            var noDraw = new Vector3(int.MinValue, int.MinValue, int.MinValue);
            // Gizmos.color = Color.red;
            for (int i = 0, len = points.Count; i < len - 1; i++)
            {
                // i = points[i + 1] == noDraw ? i + 2 : i;
                // if(points[i] == noDraw) continue;
                i = points[i + 1] != noDraw ? i : i + 2 < len ? i + 2 : i;
                Gizmos.DrawLine(points[i], points[i + 1]);
            }
        }
    }
}
