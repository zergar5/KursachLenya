using KursachLenya.Calculus;
using KursachLenya.Core;
using KursachLenya.Core.Base;
using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;
using KursachLenya.FEM.Assembling.Local;
using KursachLenya.FEM.Parameters;
using KursachLenya.GridGenerator.Area.Core;
using KursachLenya.TwoDimensional.Parameters;

namespace KursachLenya.TwoDimensional.Assembling.Local;

public class LocalAssembler : ILocalAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly ILocalMatrixAssembler _localMatrixAssembler;
    private readonly MaterialFactory _materialFactory;
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly SigmaInterpolateProvider _lambdaInterpolateProvider;
    private readonly IFunctionalParameter _functionalParameter;
    private readonly DoubleIntegralCalculator _doubleIntegralCalculator;

    public LocalAssembler
    (
        Grid<Node2D> grid,
        ILocalMatrixAssembler localMatrixAssembler,
        MaterialFactory materialFactory,
        LocalBasisFunctionsProvider localBasisFunctionsProvider,
        SigmaInterpolateProvider lambdaInterpolateProvider,
        IFunctionalParameter functionalParameter,
        DoubleIntegralCalculator doubleIntegralCalculator
    )
    {
        _grid = grid;
        _localMatrixAssembler = localMatrixAssembler;
        _materialFactory = materialFactory;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
        _lambdaInterpolateProvider = lambdaInterpolateProvider;
        _functionalParameter = functionalParameter;
        _doubleIntegralCalculator = doubleIntegralCalculator;
    }

    public LocalMatrix AssembleStiffnessMatrix(Element element)
    {
        var matrix = GetStiffnessMatrix(element);

        return new LocalMatrix(element.NodesIndexes, matrix);
    }

    public LocalMatrix AssembleMassMatrix(Element element)
    {
        var matrix = GetMassMatrix(element);

        return new LocalMatrix(element.NodesIndexes, matrix);
    }

    public LocalVector AssembleRightSide(Element element, double time)
    {
        var vector = GetRightPart(element, time);

        return new LocalVector(element.NodesIndexes, vector);
    }

    private BaseMatrix GetStiffnessMatrix(Element element)
    {
        var stiffness = _localMatrixAssembler.AssembleStiffnessMatrix(element);

        return stiffness;
    }

    private BaseMatrix GetMassMatrix(Element element)
    {
        var mass = new BaseMatrix(element.NodesIndexes.Length);

        var rInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].R, _grid.Nodes[element.NodesIndexes[2]].R);
        var zInterval = new Interval(_grid.Nodes[element.NodesIndexes[0]].Z, _grid.Nodes[element.NodesIndexes[6]].Z);

        var sigmaInterpolate = _lambdaInterpolateProvider.GetSigmaInterpolate(element);
        var biquadraticFunctions = _localBasisFunctionsProvider.GetBiquadraticFunctions();

        var nodeBilinear = new Node2D();
        var nodeBiquadratic = new Node2D();

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                mass[i, j] = _doubleIntegralCalculator.Calculate
                (
                    rInterval,
                    zInterval,
                    (r, z) =>
                    {
                        nodeBilinear.R = r;
                        nodeBilinear.Z = z;
                        nodeBiquadratic.R = (nodeBilinear.R - _grid.Nodes[element.NodesIndexes[0]].R) / element.Length;
                        nodeBiquadratic.Z = (nodeBilinear.Z - _grid.Nodes[element.NodesIndexes[0]].Z) / element.Height;
                        return sigmaInterpolate(nodeBilinear) *
                               biquadraticFunctions[i].Calculate(nodeBiquadratic) *
                               biquadraticFunctions[j].Calculate(nodeBiquadratic) *
                               r;
                    }
                );

                mass[j, i] = mass[i, j];
            }
        }

        mass = _localMatrixAssembler.AssembleMassMatrix(element);

        var sigmas = _materialFactory.GetById(element.MaterialId).Sigmas;

        BaseMatrix.Multiply((sigmas[0] + sigmas[1] + sigmas[2]) / 6 + sigmas[3] / 2, mass);

        return mass;
    }

    private BaseVector GetRightPart(Element element, double time)
    {
        var rightPart = new BaseVector(element.NodesIndexes.Length);
        var mass = _localMatrixAssembler.AssembleMassMatrix(element);

        for (var i = 0; i < rightPart.Count; i++)
        {
            rightPart[i] = _functionalParameter.Calculate(_grid.Nodes[element.NodesIndexes[i]], time);
        }

        return mass * rightPart;
    }
}