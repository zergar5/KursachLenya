using KursachLenya.Core;
using KursachLenya.Core.Global;
using KursachLenya.FEM.Assembling;
using KursachLenya.FEM.Assembling.Local;

namespace KursachLenya.TwoDimensional.Assembling.Global;

public class GlobalAssembler<TNode>
{
    private readonly IMatrixPortraitBuilder<TNode, SymmetricSparseMatrix> _matrixPortraitBuilder;
    private readonly ILocalAssembler _localAssembler;
    private readonly IInserter<SymmetricSparseMatrix> _inserter;

    public GlobalAssembler
    (
        IMatrixPortraitBuilder<TNode, SymmetricSparseMatrix> matrixPortraitBuilder,
        ILocalAssembler localAssembler,
        IInserter<SymmetricSparseMatrix> inserter
    )
    {
        _matrixPortraitBuilder = matrixPortraitBuilder;
        _localAssembler = localAssembler;
        _inserter = inserter;
    }

    public SymmetricSparseMatrix AssembleStiffnessMatrix(Grid<TNode> grid)
    {
        var globalMatrix = _matrixPortraitBuilder.Build(grid);

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleStiffnessMatrix(element);

            _inserter.InsertMatrix(globalMatrix, localMatrix);
        }

        return globalMatrix;
    }

    public SymmetricSparseMatrix AssembleMassMatrix(Grid<TNode> grid)
    {
        var globalMatrix = _matrixPortraitBuilder.Build(grid);

        foreach (var element in grid)
        {
            var localMatrix = _localAssembler.AssembleMassMatrix(element);

            _inserter.InsertMatrix(globalMatrix, localMatrix);
        }

        return globalMatrix;
    }

    public GlobalVector AssembleRightPart(Grid<TNode> grid, double time)
    {
        var rightPart = new GlobalVector(grid.Nodes.Length);

        foreach (var element in grid)
        {
            var localRightPart = _localAssembler.AssembleRightSide(element, time);

            _inserter.InsertVector(rightPart, localRightPart);
        }

        return rightPart;
    }
}