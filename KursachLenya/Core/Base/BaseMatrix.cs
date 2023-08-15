namespace KursachLenya.Core.Base;

public class BaseMatrix
{
    public double[,] Values { get; }

    public BaseMatrix() : this(new double[0, 0]) { }
    public BaseMatrix(double[,] matrix)
    {
        Values = matrix;
    }
    public BaseMatrix(int n) : this(new double[n, n]) { }

    public int CountRows => Values.GetLength(0);
    public int CountColumns => Values.GetLength(1);

    public double this[int i, int j]
    {
        get => Values[i, j];
        set => Values[i, j] = value;
    }

    public static BaseMatrix operator *(double coefficient, BaseMatrix matrix)
    {
        var localMatrix = new BaseMatrix(matrix.CountRows);

        for (var i = 0; i < localMatrix.CountRows; i++)
        {
            for (var j = 0; j < localMatrix.CountColumns; j++)
            {
                localMatrix[i, j] = coefficient * matrix[i, j];
            }
        }

        return localMatrix;
    }

    public static BaseMatrix operator *(BaseMatrix matrix, double coefficient)
    {
        return coefficient * matrix;
    }

    public static BaseMatrix operator /(BaseMatrix matrix, double coefficient)
    {
        return 1d / coefficient * matrix;
    }

    public static BaseVector operator *(BaseMatrix matrix, BaseVector vector)
    {
        var localVector = new BaseVector(vector.Count);

        if (matrix.CountRows != vector.Count)
        {
            throw new Exception("Can't multiply matrix");
        }

        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                localVector[i] += matrix[i, j] * vector[j];
            }
        }

        return localVector;
    }

    public static BaseMatrix Sum(BaseMatrix matrix1, BaseMatrix matrix2)
    {
        if (matrix1.CountRows != matrix2.CountRows && matrix1.CountColumns != matrix2.CountColumns)
        {
            throw new Exception("Can't sum matrix");
        }

        for (var i = 0; i < matrix1.CountRows; i++)
        {
            for (var j = 0; j < matrix1.CountColumns; j++)
            {
                matrix1[i, j] += matrix2[i, j];
            }
        }

        return matrix1;
    }

    public static BaseMatrix Multiply(double coefficient, BaseMatrix matrix)
    {
        for (var i = 0; i < matrix.CountRows; i++)
        {
            for (var j = 0; j < matrix.CountColumns; j++)
            {
                matrix[i, j] *= coefficient;
            }
        }

        return matrix;
    }
}