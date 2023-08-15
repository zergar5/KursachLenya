using KursachLenya.Core;
using KursachLenya.Core.Boundary;
using KursachLenya.Core.GridComponents;

namespace KursachLenya.TwoDimensional.Assembling.Boundary;

public class FirstConditionProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly Func<Node2D, double, double> _u;

    public FirstConditionProvider(Grid<Node2D> grid, Func<Node2D, double, double> u)
    {
        _grid = grid;
        _u = u;
    }

    public FirstConditionValue[] GetConditions(FirstCondition[] conditions, double time)
    {
        var conditionsValues = new List<FirstConditionValue>(conditions.Length);

        foreach (var t in conditions)
        {
            var (indexes, _) = _grid.Elements[t.ElementIndex].GetBoundNodeIndexes(t.Bound);

            var values = new double[indexes.Length];

            for (var j = 0; j < indexes.Length; j++)
            {
                values[j] = Calculate(indexes[j], time);
            }

            conditionsValues.Add(new FirstConditionValue(indexes, values));
        }

        return conditionsValues.ToArray();
    }

    public FirstCondition[] GetArrays(int elementsByLength, int elementsByHeight)
    {
        var conditions = new List<FirstCondition>(2 * (elementsByLength + elementsByHeight));

        for (var i = 0; i < elementsByLength; i++)
        {
            conditions.Add(new FirstCondition(i, Bound.Lower));
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            conditions.Add(new FirstCondition(i * elementsByLength, Bound.Left));
        }

        for (var i = 0; i < elementsByHeight; i++)
        {
            conditions.Add(new FirstCondition((i + 1) * elementsByLength - 1, Bound.Right));
        }

        for (var i = elementsByLength * (elementsByHeight - 1); i < elementsByLength * elementsByHeight; i++)
        {
            conditions.Add(new FirstCondition(i, Bound.Upper));
        }

        return conditions.ToArray();
    }

    private double Calculate(int index, double time)
    {
        return _u(_grid.Nodes[index], time);
    }
}