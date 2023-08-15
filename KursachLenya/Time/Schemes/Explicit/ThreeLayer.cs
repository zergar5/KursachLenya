using KursachLenya.Core.Global;

namespace KursachLenya.Time.Schemes.Explicit;

public class ThreeLayer
{
    private readonly SymmetricSparseMatrix _stiffnessMatrix;
    private readonly SymmetricSparseMatrix _massMatrix;

    public ThreeLayer(SymmetricSparseMatrix stiffnessMatrix, SymmetricSparseMatrix massMatrix)
    {
        _stiffnessMatrix = stiffnessMatrix;
        _massMatrix = massMatrix;
    }

    public Equation<SymmetricSparseMatrix> BuildEquation
    (
        GlobalVector rightPart,
        GlobalVector previousSolution,
        GlobalVector twoLayersBackSolution,
        double currentTime,
        double previousTime,
        double twoLayersBackTime
    )
    {
        var delta01 = currentTime - previousTime;
        var delta02 = currentTime - twoLayersBackTime;
        var delta12 = previousTime - twoLayersBackTime;

        var matrixA =
            SymmetricSparseMatrix.Sum
            (
                (delta01 + delta02) / (delta01 * delta02) * _massMatrix,
                _stiffnessMatrix
            );
        var q = new GlobalVector(matrixA.Count);
        var b =
            GlobalVector.Sum
            (
                rightPart,
                GlobalVector.Subtract
                (
                    GlobalVector.Multiply(delta02 / (delta01 * delta12), _massMatrix * previousSolution),
                    GlobalVector.Multiply(delta01 / (delta02 * delta12), _massMatrix * twoLayersBackSolution)
                )
            );

        return new Equation<SymmetricSparseMatrix>(matrixA, q, b);
    }
}