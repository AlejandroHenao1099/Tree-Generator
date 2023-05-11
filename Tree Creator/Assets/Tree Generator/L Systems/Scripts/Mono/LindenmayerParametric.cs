using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LindenmayerSystems;
using System.Diagnostics;

public class LindenmayerParametric : MonoBehaviour
{
    [TextArea]
    public string axiom;
    public int derivations = 0;
    public POLRules[] POLRules;
    public char[] ignore;

    public GlobalVariable[] globalVariables;

    public TurtleParametricRules turtleRules;

    private MeshRenderer meshRenderer;
    private Texture2D texture;

    public bool PD = true;

    void Start()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var pol = new POLSystem(axiom, derivations, POLRules, ignore, globalVariables);
        if (PD)
            axiom = pol.CreateDP();
        else
            axiom = pol.Create();
        stopwatch.Stop();
        print(stopwatch.ElapsedTicks);
        byte[,] matrix;
        var turtle = new TurtleParametric2D(axiom, turtleRules);
        matrix = turtle.Create();


        MatrixToTexture(matrix);
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = texture;
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    }

    private void MatrixToTexture(byte[,] matrix)
    {
        var resolution = matrix.GetLength(0);
        Color[] colors = new Color[resolution * resolution];
        for (int i = 0, n = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                colors[n++] = matrix[i, j] == 1 ? Color.red : Color.white;

        texture = new Texture2D(resolution, resolution);
        texture.SetPixels(0, 0, resolution, resolution, colors);
    }
}
