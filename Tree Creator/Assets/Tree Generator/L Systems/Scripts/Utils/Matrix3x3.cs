using UnityEngine;

public struct Matrix3x3
{
    private Vector3 row0;
    private Vector3 row1;
    private Vector3 row2;

    public float this[int row, int column]
    {
        get
        {
            switch (row)
            {
                case 0:
                    return row0[column];
                case 1:
                    return row1[column];
                case 2:
                    return row2[column];
                default:
                    return 0;
            }
        }
        set
        {
            switch (row)
            {
                case 0:
                    row0[column] = value;
                    break;
                case 1:
                    row1[column] = value;
                    break;
                case 2:
                    row2[column] = value;
                    break;
            }
        }
    }

    private void SetValue(int row, int column, float value)
    {
        switch (row)
        {
            case 0:
                row0[column] = value;
                break;
            case 1:
                row1[column] = value;
                break;
            case 2:
                row2[column] = value;
                break;
        }
    }

    private float GetValue(int row, int column)
    {
        switch (row)
        {
            case 0:
                return row0[column];
            case 1:
                return row1[column];
            case 2:
                return row2[column];
            default:
                return 0;
        }
    }

    public Vector3 Forward
    {
        get => new Vector3(this[0, 0], this[1, 0], this[2, 0]);
    }
    public Vector3 Right
    {
        get => new Vector3(this[0, 1], this[1, 1], this[2, 1]);
    }
    public Vector3 Up
    {
        get => new Vector3(this[0, 2], this[1, 2], this[2, 2]);
    }

    public Matrix3x3(Vector3 column0, Vector3 column1, Vector3 column2)
    {
        row0 = new Vector3(column0.x, column1.x, column2.x);
        row1 = new Vector3(column0.y, column1.y, column2.y);
        row2 = new Vector3(column0.z, column1.z, column2.z);
        SetColumn(0, column0);
        SetColumn(1, column1);
        SetColumn(2, column2);
    }

    public void SetColumn(int index, Vector3 column)
    {
        this[0, index] = column.x;
        this[1, index] = column.y;
        this[2, index] = column.z;
    }

    public void SetRow(int index, Vector3 row)
    {
        this[index, 0] = row.x;
        this[index, 1] = row.y;
        this[index, 2] = row.z;
    }

    public static Vector3 operator *(Matrix3x3 matrix, Vector3 vector)
    {
        return new Vector3(
            matrix[0, 0] * vector.x + matrix[0, 1] * vector.y + matrix[0, 2] * vector.z,
            matrix[1, 0] * vector.x + matrix[1, 1] * vector.y + matrix[1, 2] * vector.z,
            matrix[2, 0] * vector.x + matrix[2, 1] * vector.y + matrix[2, 2] * vector.z
        );
    }

    public static Matrix3x3 operator *(Matrix3x3 A, Matrix3x3 B)
    {
        var matrix = new Matrix3x3(Vector3.zero, Vector3.zero, Vector3.zero);
        matrix.SetRow(0,
            new Vector3(A[0, 0] * B[0, 0] + A[0, 1] * B[1, 0] + A[0, 2] * B[2, 0],
                        A[0, 0] * B[0, 1] + A[0, 1] * B[1, 1] + A[0, 2] * B[2, 1],
                        A[0, 0] * B[0, 2] + A[0, 1] * B[1, 2] + A[0, 2] * B[2, 2]
                        ));
        matrix.SetRow(1,
            new Vector3(A[1, 0] * B[0, 0] + A[1, 1] * B[1, 0] + A[1, 2] * B[2, 0],
                        A[1, 0] * B[0, 1] + A[1, 1] * B[1, 1] + A[1, 2] * B[2, 1],
                        A[1, 0] * B[0, 2] + A[1, 1] * B[1, 2] + A[1, 2] * B[2, 2]
                        ));
        matrix.SetRow(2,
            new Vector3(A[2, 0] * B[0, 0] + A[2, 1] * B[1, 0] + A[2, 2] * B[2, 0],
                        A[2, 0] * B[0, 1] + A[2, 1] * B[1, 1] + A[2, 2] * B[2, 1],
                        A[2, 0] * B[0, 2] + A[2, 1] * B[1, 2] + A[2, 2] * B[2, 2]
                        ));
        return matrix;
    }

    public static Matrix3x3 AlignZ
    {
        get => new Matrix3x3(Vector3.forward, Vector3.right, Vector3.up);
    }

    public static Matrix3x3 AlignY
    {
        get => new Matrix3x3(Vector3.up, Vector3.right, Vector3.back);
    }

    public static Matrix3x3 AlignX
    {
        get => new Matrix3x3(Vector3.right, Vector3.back, Vector3.up);
    }
}