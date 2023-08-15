using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;

namespace KursachLenya.FEM.Assembling.Local;

public interface ILocalAssembler
{
    public LocalMatrix AssembleStiffnessMatrix(Element element);
    public LocalMatrix AssembleMassMatrix(Element element);
    public LocalVector AssembleRightSide(Element element, double timeLayer);
}