using KursachLenya.Core.Base;
using KursachLenya.FEM.Assembling;

namespace KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Mass;

public class MassMatrixRTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 1d, 0d, -1d },
                { 0d, 16d, 4d },
                { -1d, 4d, 7d }
            }
        );
    }
}