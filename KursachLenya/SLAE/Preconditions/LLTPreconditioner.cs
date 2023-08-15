using KursachLenya.Core.Global;

namespace KursachLenya.SLAE.Preconditions;

public class LLTPreconditioner : IPreconditioner<SymmetricSparseMatrix>
{
    public SymmetricSparseMatrix Decompose(SymmetricSparseMatrix globalMatrix)
    {
        var preconditionMatrix = globalMatrix.Clone();

        for (var i = 0; i < preconditionMatrix.Count; i++)
        {
            var sumD = 0.0;
            for (var j = preconditionMatrix.RowsIndexes[i]; j < preconditionMatrix.RowsIndexes[i + 1]; j++)
            {
                var sum = 0d;

                for (var k = preconditionMatrix.RowsIndexes[i]; k < j; k++)
                {
                    var iPrev = i - preconditionMatrix.ColumnsIndexes[j];
                    var kPrev = preconditionMatrix[i - iPrev, preconditionMatrix.ColumnsIndexes[k]];

                    if (kPrev == -1) continue;

                    sum += preconditionMatrix.Values[k] * preconditionMatrix.Values[kPrev];
                }

                preconditionMatrix.Values[j] = (preconditionMatrix.Values[j] - sum) / preconditionMatrix.Diagonal[preconditionMatrix.ColumnsIndexes[j]];

                sumD += Math.Pow(preconditionMatrix.Values[j], 2);
            }

            preconditionMatrix.Diagonal[i] = Math.Sqrt(preconditionMatrix.Diagonal[i] - sumD);
        }

        return preconditionMatrix;
    }
}