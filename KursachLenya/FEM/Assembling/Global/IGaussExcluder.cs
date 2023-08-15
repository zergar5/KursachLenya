using KursachLenya.Core.Boundary;
using KursachLenya.Core.Global;

namespace KursachLenya.FEM.Assembling.Global;

public interface IGaussExcluder<TMatrix>
{
    public void Exclude(Equation<TMatrix> equation, FirstConditionValue conditionValue);
}