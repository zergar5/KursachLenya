using KursachLenya.Calculus;
using KursachLenya.Core.Boundary;
using KursachLenya.Core.GridComponents;
using KursachLenya.FEM;
using KursachLenya.GridGenerator;
using KursachLenya.GridGenerator.Area.Core;
using KursachLenya.GridGenerator.Area.Splitting;
using KursachLenya.SLAE.Preconditions;
using KursachLenya.SLAE.Solvers;
using KursachLenya.Time;
using KursachLenya.TwoDimensional;
using KursachLenya.TwoDimensional.Assembling;
using KursachLenya.TwoDimensional.Assembling.Boundary;
using KursachLenya.TwoDimensional.Assembling.Global;
using KursachLenya.TwoDimensional.Assembling.Local;
using KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Mass;
using KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Stiffness;
using KursachLenya.TwoDimensional.Parameters;
using System.Globalization;

Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

var gridBuilder2D = new GridBuilder2D();
var grid = gridBuilder2D
    .SetRAxis(new AxisSplitParameter(
            new[] { 0d, 2d },
            new QuadraticUniformSplitter(2)
        )
    )
    .SetZAxis(new AxisSplitParameter(
            new[] { 0d, 2d },
            new QuadraticUniformSplitter(2)
        )
    )
    .Build();

var materialFactory = new MaterialFactory
(
    new List<double> { 1d },
    new List<double[]> { new[] { 1d, 1d, 1d, 1d } }
);

var stiffnessMatrixTemplateProvider = new StiffnessMatrixTemplateProvider();
var stiffnessMatrixRTemplateProvider = new StiffnessMatrixRTemplateProvider();
var massMatrixTemplateProvider = new MassMatrixTemplateProvider();
var massMatrixRTemplateProvider = new MassMatrixRTemplateProvider();

var localMatrixAssembler = new LocalMatrixAssembler(grid, materialFactory,
    stiffnessMatrixTemplateProvider, stiffnessMatrixRTemplateProvider,
    massMatrixTemplateProvider, massMatrixRTemplateProvider);

var localBasisFunctionsProvider = new LocalBasisFunctionsProvider(grid, new LinearFunctionsProvider());
var sigmaInterpolateProvider = new SigmaInterpolateProvider(localBasisFunctionsProvider, materialFactory);

Func<Node2D, double, double> u = (p, t) => Math.Pow(p.R, 2) + Math.Pow(p.Z, 2) + Math.Exp(t);

var f = new RightPartParameter((p, t) => -6 + Math.Exp(t), grid);

var derivativeCalculator = new DerivativeCalculator();

var localAssembler = new LocalAssembler(grid,
    localMatrixAssembler, materialFactory, localBasisFunctionsProvider,
    sigmaInterpolateProvider, f, new DoubleIntegralCalculator());

var inserter = new Inserter();
var globalAssembler = new GlobalAssembler<Node2D>(new MatrixPortraitBuilder(), localAssembler, inserter);

var timeLayers = new ProportionalSplitter(320, 1.33)
    //new UniformSplitter(40)
    .EnumerateValues(new Interval(0, 4))
    .ToArray();

var firstBoundaryProvider = new FirstConditionProvider(grid, u);
var secondBoundaryProvider = new SecondConditionProvider(grid, materialFactory, u, derivativeCalculator, massMatrixRTemplateProvider, massMatrixTemplateProvider);
var thirdBoundaryProvider = new ThirdConditionProvider(grid, materialFactory, u, derivativeCalculator, massMatrixRTemplateProvider, massMatrixTemplateProvider);

var lltPreconditioner = new LLTPreconditioner();
var solver = new MCG(lltPreconditioner, new LLTSparse(lltPreconditioner));

var timeDiscreditor = new TimeDisсretizer(globalAssembler, firstBoundaryProvider,
    new GaussExcluder(), secondBoundaryProvider, thirdBoundaryProvider, inserter);

var solutions =
    timeDiscreditor
        .SetGrid(grid)
        .SetTimeLayers(timeLayers)
        .SetInitialSolution(u)
        .SetInitialSolution(u)
        .SetInitialSolution(u)
        //.SetSecondConditions
        //(
        //    new SecondCondition[]
        //    {
        //        new(0, Bound.Lower), new(0, Bound.Right)
        //    }
        //)
        //.SetThirdConditions
        //(
        //    new ThirdCondition[]
        //    {
        //        new(0, Bound.Lower, 1), new(0, Bound.Right, 1)
        //    }
        //)
        //.SetFirstConditions
        //(
        //    new FirstCondition[]
        //    {
        //        new(0, Bound.Left), new(0, Bound.Lower),
        //        new(1, Bound.Right), new(1, Bound.Lower),
        //        new(2, Bound.Left), new(2, Bound.Upper),
        //        new(3, Bound.Right), new(3, Bound.Upper)
        //    }
        //)
        //.SetFirstConditions
        //(
        //    new FirstCondition[]
        //    {
        //        new(0, Bound.Left), new(0, Bound.Upper)
        //    }
        //)
        .SetFirstConditions(firstBoundaryProvider.GetArrays(2, 2))
        .SetSolver(solver)
        .GetSolutions();

var femSolution = new FEMSolution(grid, solutions, timeLayers, localBasisFunctionsProvider);
var result = femSolution.Calculate(new Node2D(1.5d, 1.5d), 2);

Console.WriteLine(Math.Abs(u(new Node2D(1.5d, 1.5d), 2) - result));