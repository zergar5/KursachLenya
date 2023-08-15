using KursachLenya.Core.Global;

namespace KursachLenya.Time.Schemes.Explicit;

public class FourLayer
{
    private readonly SymmetricSparseMatrix _stiffnessMatrix;
    private readonly SymmetricSparseMatrix _massMatrix;

    public FourLayer(SymmetricSparseMatrix stiffnessMatrix, SymmetricSparseMatrix massMatrix)
    {
        _stiffnessMatrix = stiffnessMatrix;
        _massMatrix = massMatrix;
    }

    public Equation<SymmetricSparseMatrix> BuildEquation
    (
        GlobalVector rightPart,
        GlobalVector previousSolution,
        GlobalVector twoLayersBackSolution,
        GlobalVector threeLayersBackSolution,
        double currentTime,
        double previousTime,
        double twoLayersBackTime,
        double threeLayersBackTime
    )
    {
        var delta01 = currentTime - previousTime;
        var delta02 = currentTime - twoLayersBackTime;
        var delta03 = currentTime - threeLayersBackTime;
        var delta12 = previousTime - twoLayersBackTime;
        var delta13 = previousTime - threeLayersBackTime;
        var delta23 = twoLayersBackTime - threeLayersBackTime;

        var matrixA =
            SymmetricSparseMatrix.Sum
            (
                (delta01 * delta02 + delta03 * (delta01 + delta02)) / (delta01 * delta02 * delta03) * _massMatrix,
                _stiffnessMatrix
            );
        var q = new GlobalVector(matrixA.Count);
        var b =
            GlobalVector.Sum
            (
                GlobalVector.Sum
                (
                    rightPart,
                    GlobalVector.Multiply(delta02 * delta03 / (delta01 * delta12 * delta13), _massMatrix * previousSolution)
                ),
                GlobalVector.Sum
                (
                    GlobalVector.Multiply(-delta01 * delta03 / (delta02 * delta12 * delta23), _massMatrix * twoLayersBackSolution),
                    GlobalVector.Multiply(delta01 * delta02 / (delta03 * delta13 * delta23), _massMatrix * threeLayersBackSolution)
                )
            );

        return new Equation<SymmetricSparseMatrix>(matrixA, q, b);
    }
}