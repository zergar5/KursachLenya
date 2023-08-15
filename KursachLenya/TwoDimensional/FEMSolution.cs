using KursachLenya.Core;
using KursachLenya.Core.Global;
using KursachLenya.Core.GridComponents;
using KursachLenya.FEM;
using KursachLenya.GridGenerator.Area.Core;
using KursachLenya.TwoDimensional.Assembling.Local;

namespace KursachLenya.TwoDimensional;

public class FEMSolution
{
    private readonly Grid<Node2D> _grid;
    private readonly GlobalVector[] _solutions;
    private readonly double[] _timeLayers;
    private readonly LocalBasisFunctionsProvider _basisFunctionsProvider;

    public FEMSolution(Grid<Node2D> grid, GlobalVector[] solutions, double[] timeLayers, LocalBasisFunctionsProvider basisFunctionsProvider)
    {
        _grid = grid;
        _solutions = solutions;
        _timeLayers = timeLayers;
        _basisFunctionsProvider = basisFunctionsProvider;
    }

    public double Calculate(Node2D point, double time)
    {
        if (TimeLayersHas(time) && AreaHas(point))
        {
            var currentTimeLayerIndex = FindCurrentTimeLayer(time);

            var element = _grid.Elements.First(x => ElementHas(x, point));

            var basisFunctions = _basisFunctionsProvider.GetBiquadraticFunctions();

            var lagrangePolynomials = CreateLagrangePolynomials(currentTimeLayerIndex);

            var sum = 0d;
            var quadraticNode = new Node2D((point.R - _grid.Nodes[element.NodesIndexes[0]].R) / element.Length,
                (point.Z - _grid.Nodes[element.NodesIndexes[0]].Z) / element.Height);

            for (var j = 0; j < lagrangePolynomials.Length; j++)
            {
                sum += element.NodesIndexes
                    .Select((t, i) => _solutions[currentTimeLayerIndex - j][t] * basisFunctions[i].Calculate(quadraticNode))
                    .Sum() * lagrangePolynomials[j](time);
            }

            Informer.WriteSolution(point, time, sum);

            return sum;
        }

        Informer.WriteAreaInfo();
        Informer.WriteSolution(point, double.NaN, double.NaN);
        return double.NaN;
    }

    public double CalcError(Func<Node2D, double, double> u, double time)
    {
        var solution = new GlobalVector(_solutions[0].Count);
        var trueSolution = new GlobalVector(_solutions[0].Count);

        for (var i = 0; i < _solutions[0].Count; i++)
        {
            solution[i] = Calculate(_grid.Nodes[i], time);
            trueSolution[i] = u(_grid.Nodes[i], time);
        }

        GlobalVector.Subtract(solution, trueSolution);

        return trueSolution.Norm;
    }

    private bool ElementHas(Element element, Node2D node)
    {
        var leftCornerNode = _grid.Nodes[element.NodesIndexes[0]];
        var rightCornerNode = _grid.Nodes[element.NodesIndexes[^1]];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }

    private bool AreaHas(Node2D node)
    {
        var leftCornerNode = _grid.Nodes[0];
        var rightCornerNode = _grid.Nodes[^1];
        return node.R >= leftCornerNode.R && node.Z >= leftCornerNode.Z &&
               node.R <= rightCornerNode.R && node.Z <= rightCornerNode.Z;
    }

    private bool TimeLayersHas(double time)
    {
        var interval = new Interval(_timeLayers[0], _timeLayers[^1]);
        return interval.Has(time);
    }

    private int FindCurrentTimeLayer(double time)
    {
        return Array.FindIndex(_timeLayers, x => time <= x);
    }

    private Func<double, double>[] CreateLagrangePolynomials(int timeLayerIndex)
    {
        switch (timeLayerIndex)
        {
            case 1:
                {
                    var currentTime = _timeLayers[timeLayerIndex];
                    var previousTime = _timeLayers[timeLayerIndex - 1];
                    var values = new Func<double, double>[]
                    {
                        t => (t - previousTime) / (currentTime - previousTime),
                        t => (t - currentTime) / (previousTime - currentTime)
                    };

                    return values;
                }
            case 2:
                {
                    var currentTime = _timeLayers[timeLayerIndex];
                    var previousTime = _timeLayers[timeLayerIndex - 1];
                    var twoLayersBackTime = _timeLayers[timeLayerIndex - 2];
                    var values = new Func<double, double>[]
                    {
                        t => (t - twoLayersBackTime) * (t - previousTime) /
                            ((currentTime - twoLayersBackTime) * (currentTime - previousTime)),
                        t => -(t - twoLayersBackTime) * (t - currentTime) /
                            ((currentTime - previousTime) * (previousTime - twoLayersBackTime)),
                        t => (t - previousTime) * (t - currentTime) /
                            ((currentTime - twoLayersBackTime) * (previousTime - twoLayersBackTime)),
                    };

                    return values;
                }
            default:
                {
                    var currentTime = _timeLayers[timeLayerIndex];
                    var previousTime = _timeLayers[timeLayerIndex - 1];
                    var twoLayersBackTime = _timeLayers[timeLayerIndex - 2];
                    var threeLayersBackTime = _timeLayers[timeLayerIndex - 3];

                    var values = new Func<double, double>[]
                    {
                        t => (t - threeLayersBackTime) * (t - twoLayersBackTime) * (t - previousTime) /
                            ((currentTime - threeLayersBackTime) * (currentTime - twoLayersBackTime) *
                            (currentTime - previousTime)),
                        t => (t - threeLayersBackTime) * (t - twoLayersBackTime) * (t - currentTime) /
                            ((previousTime - threeLayersBackTime) * (previousTime - twoLayersBackTime) *
                            (previousTime - currentTime)),
                        t => (t - threeLayersBackTime) * (t - previousTime) * (t - currentTime) /
                            ((twoLayersBackTime - threeLayersBackTime) * (twoLayersBackTime - previousTime) *
                            (twoLayersBackTime - currentTime)),
                        t => (t - twoLayersBackTime) * (t - previousTime) * (t - currentTime) /
                            ((threeLayersBackTime - twoLayersBackTime) * (threeLayersBackTime - previousTime) *
                            (threeLayersBackTime - currentTime))
                    };

                    return values;
                }
        }
    }
}