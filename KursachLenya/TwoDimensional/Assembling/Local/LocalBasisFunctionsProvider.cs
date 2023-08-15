using KursachLenya.Core;
using KursachLenya.Core.GridComponents;
using KursachLenya.FEM;

namespace KursachLenya.TwoDimensional.Assembling.Local;

public class LocalBasisFunctionsProvider
{
    private readonly Grid<Node2D> _grid;
    private readonly LinearFunctionsProvider _linearFunctionsProvider;

    public LocalBasisFunctionsProvider(Grid<Node2D> grid, LinearFunctionsProvider linearFunctionsProvider)
    {
        _grid = grid;
        _linearFunctionsProvider = linearFunctionsProvider;
    }

    public LocalBasisFunction[] GetBilinearFunctions(Element element)
    {
        var firstXFunction =
            _linearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodesIndexes[2]].R, element.Length);
        var secondXFunction =
            _linearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodesIndexes[0]].R, element.Length);
        var firstYFunction =
            _linearFunctionsProvider.CreateFirstFunction(_grid.Nodes[element.NodesIndexes[6]].Z, element.Height);
        var secondYFunction =
            _linearFunctionsProvider.CreateSecondFunction(_grid.Nodes[element.NodesIndexes[0]].Z, element.Height);

        var basisFunctions = new LocalBasisFunction[]
        {
            new (firstXFunction, firstYFunction),
            new (secondXFunction, firstYFunction),
            new (firstXFunction, secondYFunction),
            new (secondXFunction, secondYFunction)
        };

        return basisFunctions;
    }

    public LocalBasisFunction[] GetBiquadraticFunctions()
    {
        var firstFunction = new Func<double, double>(c => 2 * (c - 0.5) * (c - 1));
        var secondFunction = new Func<double, double>(c => -4 * c * (c - 1));
        var thirdFunction = new Func<double, double>(c => 2 * c * (c - 0.5));

        var localBasisFunctions = new LocalBasisFunction[]
        {
            new (firstFunction, firstFunction),
            new (secondFunction, firstFunction),
            new (thirdFunction, firstFunction),
            new (firstFunction, secondFunction),
            new (secondFunction, secondFunction),
            new (thirdFunction, secondFunction),
            new (firstFunction, thirdFunction),
            new (secondFunction, thirdFunction),
            new (thirdFunction, thirdFunction)
        };

        return localBasisFunctions;
    }
}