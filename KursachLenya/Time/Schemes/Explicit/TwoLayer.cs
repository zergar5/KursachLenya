using KursachLenya.Core.Global;

namespace KursachLenya.Time.Schemes.Explicit;

public class TwoLayer
{
    private readonly SymmetricSparseMatrix _stiffnessMatrix;
    private readonly SymmetricSparseMatrix _massMatrix;

    public TwoLayer(SymmetricSparseMatrix stiffnessMatrix, SymmetricSparseMatrix massMatrix)
    {
        _stiffnessMatrix = stiffnessMatrix;
        _massMatrix = massMatrix;
    }

    public Equation<SymmetricSparseMatrix> BuildEquation
    (
        GlobalVector rightPart,
        GlobalVector previousSolution,
        double currentTime,
        double previousTime
    )
    {
        var delta = currentTime - previousTime;

        var deltaMassMatrix = 1d / delta * _massMatrix;

        var b =
            GlobalVector.Sum
            (
                rightPart,
                deltaMassMatrix * previousSolution
            );
        var q = new GlobalVector(b.Count);
        var matrixA =
            SymmetricSparseMatrix.Sum
            (
                deltaMassMatrix,
                _stiffnessMatrix
            );

        return new Equation<SymmetricSparseMatrix>(matrixA, q, b);
    }
}