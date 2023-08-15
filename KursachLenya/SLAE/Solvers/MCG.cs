using KursachLenya.Core.Global;
using KursachLenya.SLAE.Preconditions;

namespace KursachLenya.SLAE.Solvers;

public class MCG : ISolver<SymmetricSparseMatrix>
{
    private readonly LLTPreconditioner _lltPreconditioner;
    private readonly LLTSparse _lltSparse;
    private SymmetricSparseMatrix _preconditionMatrix;
    private GlobalVector _r;
    private GlobalVector _z;

    public MCG(LLTPreconditioner lltPreconditioner, LLTSparse lltSparse)
    {
        _lltPreconditioner = lltPreconditioner;
        _lltSparse = lltSparse;
    }

    private void PrepareProcess(Equation<SymmetricSparseMatrix> equation)
    {
        _preconditionMatrix = _lltPreconditioner.Decompose(equation.Matrix);
        _r = GlobalVector.Subtract(equation.RightSide, equation.Matrix * equation.Solution);
        _z = _lltSparse.Solve(_preconditionMatrix, _r);
    }

    public GlobalVector Solve(Equation<SymmetricSparseMatrix> equation)
    {
        PrepareProcess(equation);
        IterationProcess(equation);
        return equation.Solution;
    }

    private void IterationProcess(Equation<SymmetricSparseMatrix> equation)
    {
        var x = equation.Solution;

        var bNorm = equation.RightSide.Norm;
        var residual = _r.Norm / bNorm;

        for (var i = 1; i <= MethodsConfig.MaxIterations && residual > Math.Pow(MethodsConfig.Eps, 2); i++)
        {
            var scalarMrR = GlobalVector.ScalarProduct(_lltSparse.Solve(_preconditionMatrix, _r), _r);

            var AxZ = equation.Matrix * _z;

            var alphaK = scalarMrR / GlobalVector.ScalarProduct(AxZ, _z);

            GlobalVector.Sum(x, alphaK * _z);

            var rNext = GlobalVector.Subtract(_r, GlobalVector.Multiply(alphaK, AxZ));

            var betaK = GlobalVector.ScalarProduct(_lltSparse.Solve(_preconditionMatrix, rNext), rNext) / scalarMrR;

            var zNext = GlobalVector.Sum(_lltSparse.Solve(_preconditionMatrix, rNext), GlobalVector.Multiply(betaK, _z));

            residual = rNext.Norm / bNorm;

            _r = rNext;
            _z = zNext;

            //CourseHolder.GetInfo(i, residual);
        }

        //Console.WriteLine();
    }
}