using KursachLenya.Calculus;
using KursachLenya.Core;
using KursachLenya.Core.Base;
using KursachLenya.Core.Boundary;
using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;
using KursachLenya.FEM.Assembling;
using KursachLenya.TwoDimensional.Parameters;

namespace KursachLenya.TwoDimensional.Assembling.Boundary;

public class SecondConditionProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    private readonly Func<Node2D, double, double> _u;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly BaseMatrix _templateMatrixR;
    private readonly BaseMatrix _templateMatrix;

    public SecondConditionProvider
    (
        Grid<Node2D> grid,
        MaterialFactory materialFactory,
        Func<Node2D, double, double> u,
        DerivativeCalculator derivativeCalculator,
        ITemplateMatrixProvider templateMatrixRProvider,
        ITemplateMatrixProvider templateMatrixProvider
    )
    {
        _grid = grid;
        _materialFactory = materialFactory;
        _u = u;
        _derivativeCalculator = derivativeCalculator;
        _templateMatrixR = templateMatrixRProvider.GetMatrix();
        _templateMatrix = templateMatrixProvider.GetMatrix();
    }

    public SecondConditionValue[] GetConditions(SecondCondition[] conditions, double time)
    {
        var conditionValues = new List<SecondConditionValue>(conditions.Length);

        foreach (var t in conditions)
        {
            var (indexes, h) = _grid.Elements[t.ElementIndex].GetBoundNodeIndexes(t.Bound);

            BaseVector vector;

            var lambda = _materialFactory.GetById(_grid.Elements[t.ElementIndex].MaterialId).Lambda;

            if (t.Bound is Bound.Left or Bound.Right)
            {
                vector = GetRVector(indexes, t.Bound, h, lambda, time);
            }
            else
            {
                vector = GetZVector(indexes, t.Bound, h, lambda, time);
            }

            conditionValues.Add(new SecondConditionValue(new LocalVector(indexes, vector)));
        }

        return conditionValues.ToArray();
    }

    private BaseVector GetRVector(int[] indexes, Bound bound, double h, double lambda, double time)
    {
        var vector = new BaseVector(indexes.Length);

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = _derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], time, 'r');
        }

        if (bound == Bound.Left)
        {
            BaseVector.Multiply(-lambda, vector);
        }
        else
        {
            BaseVector.Multiply(lambda, vector);
        }

        vector = BaseVector.Multiply(h * _grid.Nodes[indexes[0]].R / 30d, _templateMatrix * vector);

        return vector;
    }

    private BaseVector GetZVector(int[] indexes, Bound bound, double h, double lambda, double time)
    {
        var vector = new BaseVector(indexes.Length);

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = _derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], time, 'z');
        }

        if (bound == Bound.Lower)
        {
            BaseVector.Multiply(-lambda, vector);
        }
        else
        {
            BaseVector.Multiply(lambda, vector);
        }

        vector = BaseMatrix.Sum
        (
            h * _grid.Nodes[indexes[0]].R / 30d  * _templateMatrix,
            Math.Pow(h, 2) / 60d * _templateMatrixR
        ) * vector;

        return vector;
    }
}