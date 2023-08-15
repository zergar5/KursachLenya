using KursachLenya.Core.Base;
using KursachLenya.FEM.Assembling;

namespace KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Stiffness;

public class StiffnessMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 7d, -8d, 1d },
                { -8d, 16d, -8d },
                { 1d, -8d, 7d }
            }
        );
    }
}