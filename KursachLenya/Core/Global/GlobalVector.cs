namespace KursachLenya.Core.Global;

public class GlobalVector
{
    public double[] Vector { get; }

    public GlobalVector()
    {
        Vector = Array.Empty<double>();
    }

    public GlobalVector(int size)
    {
        Vector = new double[size];
    }

    public GlobalVector(double[] vector)
    {
        Vector = vector;
    }

    public double this[int index]
    {
        get => Vector[index];
        set => Vector[index] = value;
    }

    public int Count => Vector.Length;
    public double Norm => Math.Sqrt(ScalarProduct(this, this));

    public static GlobalVector operator *(double coefficient, GlobalVector globalVector)
    {
        var vector = new GlobalVector(globalVector.Count);

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = coefficient * globalVector[i];
        }

        return vector;
    }

    public GlobalVector Clone()
    {
        var clone = new double[Count];
        Array.Copy(Vector, clone, Count);

        return new GlobalVector(clone);
    }

    public static double ScalarProduct(GlobalVector globalVector1, GlobalVector globalVector2)
    {
        var result = 0d;
        for (var i = 0; i < globalVector1.Count; i++)
        {
            result += globalVector1[i] * globalVector2[i];
        }
        return result;
    }

    public double ScalarProduct(GlobalVector vector)
    {
        return ScalarProduct(this, vector);
    }

    public static GlobalVector Sum(GlobalVector localVector1, GlobalVector localVector2)
    {
        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            localVector1[i] += localVector2[i];
        }

        return localVector1;
    }

    public static GlobalVector Subtract(GlobalVector localVector1, GlobalVector localVector2)
    {
        if (localVector1.Count != localVector2.Count) throw new Exception("Can't sum vectors");

        for (var i = 0; i < localVector1.Count; i++)
        {
            localVector2[i] = localVector1[i] - localVector2[i];
        }

        return localVector2;
    }

    public static GlobalVector Multiply(double number, GlobalVector vector)
    {
        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] *= number;
        }

        return vector;
    }

    public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)Vector).GetEnumerator();
}