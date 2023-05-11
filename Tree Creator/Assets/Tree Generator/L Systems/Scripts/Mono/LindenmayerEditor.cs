using System.Diagnostics;
using UnityEngine;
using LindenmayerSystems;

public class LindenmayerEditor : MonoBehaviour
{
    [TextArea]
    public string axiom;
    public int derivations = 0;
    public DOLRules[] DOLRules;
    public char[] ignore;
    public TurtleRules2D turtleRule;
    public Color color;
    public LindenmayerType lindenmayerType;

    private MeshRenderer meshRenderer;
    private Texture2D texture;
    public bool memeoizacion = true;

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Stopwatch timer = new Stopwatch();
        timer.Start();

        var dol = new DOLSystem(axiom, derivations, DOLRules, ignore);
        if (memeoizacion)
            axiom = dol.Create();
        else
            axiom = dol.NaiveCreate();

        byte[,] matrix;
        if (lindenmayerType == LindenmayerType.Lindenmayer2D)
            matrix = Lindenmayer2D.Create(axiom, turtleRule);
        else
            matrix = LindenmayerPlants2D.Create(axiom, turtleRule);

        MatrixToTexture(matrix);
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material.mainTexture = texture;
        texture.filterMode = FilterMode.Point;
        texture.Apply();

        timer.Stop();
        print(timer.ElapsedTicks);
        // 310 657 57 19
        // Olguita
    }

    private void MatrixToTexture(byte[,] matrix)
    {
        var resolution = matrix.GetLength(0);
        Color[] colors = new Color[resolution * resolution];
        for (int i = 0, n = 0; i < resolution; i++)
            for (int j = 0; j < resolution; j++)
                colors[n++] = matrix[i, j] == 1 ? color : Color.white;

        texture = new Texture2D(resolution, resolution);
        texture.SetPixels(0, 0, resolution, resolution, colors);
    }
}

public enum LindenmayerType
{
    Lindenmayer2D, LindenmayerPlants2D
}