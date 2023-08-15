using KursachLenya.Core.Base;

namespace KursachLenya.FEM.Assembling;

public interface ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix();
}