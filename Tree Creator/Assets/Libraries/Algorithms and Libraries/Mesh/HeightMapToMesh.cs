using UnityEngine;

public enum TypeVertex
{
    Shared, Triangle
}

public static class HeightMapToMesh
{
    private const byte MAX_SIZE = 129;

    public static Mesh Generate(float[,] heightMap, int scale)
    {
        Mesh mesh = new Mesh();
        mesh.name = "Map";

        var sizeMap = Mathf.Min(MAX_SIZE, heightMap.GetLength(0));
        var subSize = sizeMap - 1;
        Vector3[] vertices = new Vector3[sizeMap * sizeMap];
        int[] triangles = new int[subSize * subSize * 6];

        // if (typeVertex == TypeVertex.Shared)
        //     Shared(heightMap, vertices, triangles, sizeMap, scale);
        // else
        //     Solo(heightMap, vertices, triangles, sizeMap, scale);

        SharedVertices(vertices, heightMap, sizeMap, scale);
        SharedTriangles(triangles, subSize);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        return mesh;
    }

    public static void Generate(float[,] heightMap, Mesh mesh, Vector3[] vertices, int scale)
    {
        var sizeMap = Mathf.Min(MAX_SIZE, heightMap.GetLength(0));
        var subSize = sizeMap - 1;
        vertices = new Vector3[sizeMap * sizeMap];
        int[] triangles = new int[subSize * subSize * 6];
        SharedVertices(vertices, heightMap, sizeMap, scale);
        SharedTriangles(triangles, subSize);
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    private static int ResolutionVertices(TypeVertex typeVertex, int resolution)
    {
        switch (typeVertex)
        {
            case TypeVertex.Shared: return resolution * resolution;
            case TypeVertex.Triangle: return 6 * (resolution - 1) * (resolution - 1);
            default: return 1;
        }
    }

    private static void Shared(float[,] heightMap, Vector3[] vertices, int[] triangles, int size, float scale = 1)
    {
        SharedVertices(vertices, heightMap, size, scale);
        SharedTriangles(triangles, size - 1);
    }

    private static void Solo(float[,] heightMap, Vector3[] vertices, int[] triangles, int size, float scale = 1)
    {
        for (int i = 0, k = 0, ti = 0; i < size - 1; i++)
        {
            for (int j = 0; j < size - 1; j++)
            {
                vertices[k++] = new Vector3(i, heightMap[i, j], j);
                vertices[k++] = new Vector3(i, heightMap[i, j + 1], j + 1);
                vertices[k++] = new Vector3(i + 1, heightMap[i + 1, j], j);
                triangles[ti++] = k - 3;
                triangles[ti++] = k - 2;
                triangles[ti++] = k - 1;

                vertices[k++] = new Vector3(i, heightMap[i, j + 1], j + 1);
                vertices[k++] = new Vector3(i + 1, heightMap[i + 1, j + 1], j + 1);
                vertices[k++] = new Vector3(i + 1, heightMap[i + 1, j], j);
                triangles[ti++] = k - 3;
                triangles[ti++] = k - 2;
                triangles[ti++] = k - 1;
            }
        }
    }

    private static void SharedVertices(Vector3[] vertices, float[,] heightMap, int size, float scale = 1)
    {
        for (int i = 0, k = 0; i < size; i++)
            for (int j = 0; j < size; j++)
                vertices[k++] = new Vector3(i * scale, heightMap[i, j], j * scale);
    }

    public static Mesh[] GenerateMeshes(float[,] heightMap, int resolutionChunk, int scale = 1, int LOD = 1)
    {
        int stepVertex = LOD == 0 ? 1 : LOD * 2;
        int lodResolution = ((resolutionChunk - 1) / stepVertex) + 1;

        int numberChunks = heightMap.GetLength(0) / resolutionChunk;
        int subSizeChunks = lodResolution - 1;

        Mesh[] meshes = new Mesh[numberChunks * numberChunks];
        Vector3[] vertices = new Vector3[lodResolution * lodResolution];
        int[] triangles = new int[subSizeChunks * subSizeChunks * 6];

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i] = new Mesh();
            meshes[i].name = "Terrain";
        }

        for (int n = 0, ind = 0; n < numberChunks; n++)
        {
            for (int m = 0; m < numberChunks; m++)
            {
                SetVertices(heightMap, vertices, resolutionChunk, scale, n, m, stepVertex);
                SharedTriangles(triangles, subSizeChunks);
                // GenerarUV();
                meshes[ind].vertices = vertices;
                meshes[ind].triangles = triangles;
                meshes[ind].RecalculateNormals();
                // meshes[ind].uv = uv;
                ind++;
            }
        }
        return meshes;
    }

    private static void SetVertices(float[,] heightMap, Vector3[] vertices, int sizeChunk, int scale, int n, int m, int step = 1)
    {
        int multiN = n * (sizeChunk - 1);
        int multiM = m * (sizeChunk - 1);
        for (int i = 0, k = 0; i < sizeChunk; i += step)
        {
            for (int j = 0; j < sizeChunk; j += step)
            {
                float height = heightMap[i + multiN, j + multiM];
                vertices[k++] = new Vector3(i, height, j) * scale;
            }
        }
    }

    private static void SharedTriangles(int[] triangles, int subSize)
    {
        for (int ti = 0, vi = 0, y = 0; y < subSize; y++, vi++)
        {
            for (int x = 0; x < subSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 1] = triangles[ti + 3] = vi + 1;
                triangles[ti + 2] = triangles[ti + 5] = vi + subSize + 1;
                triangles[ti + 4] = vi + subSize + 2;
            }
        }
    }

    // private static void GenerarUV()
    // {
    //     float height;
    //     int len = vertices.Length;
    //     uv = new Vector2[len];
    //     for (int i = 0; i < len; i++)
    //     {
    //         height = vertices[i].y;
    //         if (height > 40 * escala)
    //             uv[i] = new Vector2(0.3f, 0.7f);
    //         else if (height > 20 * escala)
    //             uv[i] = new Vector2(0.7f, 0.7f);
    //         else if (height > 0)
    //             uv[i] = new Vector2(0.7f, 0.3f);
    //         else
    //             uv[i] = new Vector2(0.3f, 0.3f);
    //     }
    // }
}