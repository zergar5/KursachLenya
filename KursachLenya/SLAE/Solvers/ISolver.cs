using KursachLenya.Core.Global;

namespace KursachLenya.SLAE.Solvers;

public interface ISolver<TMatrix>
{
    public GlobalVector Solve(Equation<TMatrix> equation);
}