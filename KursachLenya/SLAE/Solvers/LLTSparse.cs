using KursachLenya.Core.Global;
using KursachLenya.SLAE.Preconditions;

namespace KursachLenya.SLAE.Solvers;

public class LLTSparse : ISolver<SymmetricSparseMatrix>
{
    private readonly LLTPreconditioner _lltPreconditioner;

    public LLTSparse(LLTPreconditioner lltPreconditioner)
    {
        _lltPreconditioner = lltPreconditioner;
    }

    public GlobalVector Solve(Equation<SymmetricSparseMatrix> equation)
    {
        var matrix = _lltPreconditioner.Decompose(equation.Matrix);
        var y = CalcY(matrix, equation.RightSide);
        var x = CalcX(matrix, y);

        return x;
    }

    public GlobalVector Solve(SymmetricSparseMatrix matrix, GlobalVector vector)
    {
        var y = CalcY(matrix, vector);
        var x = CalcX(matrix, y);

        return x;
    }

    public GlobalVector CalcY(SymmetricSparseMatrix sparseMatrix, GlobalVector b)
    {
        var y = b.Clone();

        for (var i = 0; i < sparseMatrix.Count; i++)
        {
            var sum = 0.0;
            for (var j = sparseMatrix.RowsIndexes[i]; j < sparseMatrix.RowsIndexes[i + 1]; j++)
            {
                sum += sparseMatrix.Values[j] * y[sparseMatrix.ColumnsIndexes[j]];
            }
            y[i] = (b[i] - sum) / sparseMatrix.Diagonal[i];
        }

        return y;
    }

    public GlobalVector CalcX(SymmetricSparseMatrix sparseMatrix, GlobalVector y)
    {
        var x = y;

        for (var i = sparseMatrix.Count - 1; i >= 0; i--)
        {
            x[i] /= sparseMatrix.Diagonal[i];
            for (var j = sparseMatrix.RowsIndexes[i + 1] - 1; j >= sparseMatrix.RowsIndexes[i]; j--)
            {
                x[sparseMatrix.ColumnsIndexes[j]] -= sparseMatrix.Values[j] * x[i];
            }
        }

        return x;
    }
}