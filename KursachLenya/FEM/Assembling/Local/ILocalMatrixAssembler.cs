using KursachLenya.Core.Base;
using KursachLenya.Core.GridComponents;

namespace KursachLenya.FEM.Assembling.Local;

public interface ILocalMatrixAssembler
{
    public BaseMatrix AssembleStiffnessMatrix(Element element);
    public BaseMatrix AssembleMassMatrix(Element element);
}