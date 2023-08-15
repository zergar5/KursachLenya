using KursachLenya.Calculus;
using KursachLenya.Core;
using KursachLenya.Core.Base;
using KursachLenya.Core.Boundary;
using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;
using KursachLenya.FEM.Assembling;
using KursachLenya.TwoDimensional.Parameters;

namespace KursachLenya.TwoDimensional.Assembling.Boundary;

public class ThirdConditionProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    private readonly Func<Node2D, double, double> _u;
    private readonly DerivativeCalculator _derivativeCalculator;
    private readonly BaseMatrix _templateMatrixR;
    private readonly BaseMatrix _templateMatrix;

    public ThirdConditionProvider
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

    public ThirdConditionValue[] GetConditions(ThirdCondition[] conditions, double time)
    {
        var conditionValues = new List<ThirdConditionValue>(conditions.Length);

        foreach (var t in conditions)
        {
            var (indexes, h) = _grid.Elements[t.ElementIndex].GetBoundNodeIndexes(t.Bound);
            var lambda = _materialFactory.GetById(_grid.Elements[t.ElementIndex].MaterialId).Lambda;

            BaseVector vector;
            BaseMatrix matrix;

            if (t.Bound is Bound.Left or Bound.Right)
            {
                var uS = GetUs(indexes, t.Bound, lambda, t.Beta, time, 'r');
                matrix = GetRMatrix(indexes, h, t.Beta);
                vector = matrix * uS;
            }
            else
            {
                var uS = GetUs(indexes, t.Bound, lambda, t.Beta, time, 'z');
                matrix = GetZMatrix(indexes, h, t.Beta);
                vector = matrix * uS;
            }

            conditionValues.Add(new ThirdConditionValue(new LocalMatrix(indexes, matrix),
                new LocalVector(indexes, vector)));
        }

        return conditionValues.ToArray();
    }

    private BaseMatrix GetRMatrix(int[] indexes, double h, double beta)
    {
        var matrix = beta * h * _grid.Nodes[indexes[0]].R / 30d * _templateMatrix;

        return matrix;
    }

    private BaseMatrix GetZMatrix(int[] indexes, double h, double beta)
    {
        var matrix = BaseMatrix.Sum
        (
            beta * h * _grid.Nodes[indexes[0]].R / 30d * _templateMatrix,
            beta * Math.Pow(h, 2) / 60d * _templateMatrixR
        );

        return matrix;
    }

    private BaseVector GetUs(int[] indexes, Bound bound, double lambda, double beta, double time, char variable)
    {
        var vector = new BaseVector(indexes.Length);

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = _derivativeCalculator.Calculate(_u, _grid.Nodes[indexes[i]], time, variable);
        }

        if (bound is Bound.Left or Bound.Lower)
        {
            BaseVector.Multiply(-lambda, vector);
        }
        else
        {
            BaseVector.Multiply(lambda, vector);
        }

        for (var i = 0; i < vector.Count; i++)
        {
            vector[i] = (vector[i] + beta * _u(_grid.Nodes[indexes[i]], time)) / beta;
        }

        return vector;
    }
}