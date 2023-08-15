using KursachLenya.Core.Boundary;
using KursachLenya.Core.Global;
using KursachLenya.FEM.Assembling.Global;

namespace KursachLenya.TwoDimensional.Assembling.Global;

public class GaussExcluder : IGaussExcluder<SymmetricSparseMatrix>
{
    public void Exclude(Equation<SymmetricSparseMatrix> equation, FirstConditionValue conditionValue)
    {
        for (var i = 0; i < conditionValue.Values.Length; i++)
        {
            equation.RightSide[conditionValue.NodesIndexes[i]] = conditionValue.Values[i];
            equation.Matrix.Diagonal[conditionValue.NodesIndexes[i]] = 1d;

            for (var j = equation.Matrix.RowsIndexes[conditionValue.NodesIndexes[i]];
                 j < equation.Matrix.RowsIndexes[conditionValue.NodesIndexes[i] + 1];
                 j++)
            {
                equation.RightSide[equation.Matrix.ColumnsIndexes[j]] -= equation.Matrix.Values[j] * conditionValue.Values[i];
                equation.Matrix.Values[j] = 0d;
            }

            for (var j = conditionValue.NodesIndexes[i] + 1; j < equation.Matrix.Count; j++)
            {
                var elementIndex = equation.Matrix[j, conditionValue.NodesIndexes[i]];

                if (elementIndex == -1) continue;

                equation.RightSide[j] -= equation.Matrix.Values[elementIndex] * conditionValue.Values[i];
                equation.Matrix.Values[elementIndex] = 0d;
            }
        }
    }
}