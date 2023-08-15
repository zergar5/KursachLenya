using KursachLenya.Core.Base;
using KursachLenya.FEM.Assembling;

namespace KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Stiffness;

public class StiffnessMatrixRTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 3d, -4d, 1d },
                { -4d, 16d, -12d },
                { 1d, -12d, 11d }
            }
        );
    }
}