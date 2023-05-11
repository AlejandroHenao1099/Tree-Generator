using System.Collections.Generic;
using UnityEngine;

public static class LineOnMatrix
{
    public static void DrawLine(byte[,] matrix, List<Vector2Int> points)
    {
        for (int i = 0, len = points.Count; i < len - 1; i++)
            DrawLine(matrix, points[i], points[i + 1]);
    }

    public static void DrawLine(byte[,] matrix, Vector2Int from, Vector2Int to)
    {
        if (from.x < 0 || to.x < 0 || from.y < 0 || to.y < 0)
            return;

        var start = from.x < to.x || from.y < to.y ? from : to;
        var end = start == from ? to : from;

        if (start.x == end.x)
        {
            for (int i = start.y; i < end.y; i++)
                matrix[i, start.x] = 1;
            return;
        }

        if (Mathf.Abs(end.y - start.y) < Mathf.Abs(end.x - start.x))
        {
            if (start.x > end.x)
                LineLow(matrix, end, start);
            else
                LineLow(matrix, start, end);
        }
        else
        {
            if (start.y > end.y)
                LineHigh(matrix, end, start);
            else
                LineHigh(matrix, start, end);
        }
    }

    private static void LineLow(byte[,] matrix, Vector2Int start, Vector2Int end)
    {
        var dx = end.x - start.x;
        var dy = end.y - start.y;
        var yi = 1;
        if (dy < 0)
        {
            yi = -1;
            dy = -dy;
        }
        var D = (2 * dy) - dx;
        var y = start.y;

        for (int x = start.x; x <= end.x; x++)
        {
            matrix[y, x] = 1;
            if (D > 0)
            {
                y = y + yi;
                D = D + (2 * (dy - dx));
            }
            else
                D = D + 2 * dy;
        }
    }

    private static void LineHigh(byte[,] matrix, Vector2Int start, Vector2Int end)
    {
        var dx = end.x - start.x;
        var dy = end.y - start.y;
        var xi = 1;
        if (dx < 0)
        {
            xi = -1;
            dx = -dx;
        }
        var D = (2 * dx) - dy;
        var x = start.x;
        for (int y = start.y; y <= end.y; y++)
        {
            matrix[y, x] = 1;
            if (D > 0)
            {
                x = x + xi;
                D = D + (2 * (dx - dy));
            }
            else
                D = D + 2 * dx;
        }
    }
}
