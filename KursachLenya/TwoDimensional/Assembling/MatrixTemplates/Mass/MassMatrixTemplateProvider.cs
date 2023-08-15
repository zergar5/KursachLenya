using KursachLenya.Core.Base;
using KursachLenya.FEM.Assembling;

namespace KursachLenya.TwoDimensional.Assembling.MatrixTemplates.Mass;

public class MassMatrixTemplateProvider : ITemplateMatrixProvider
{
    public BaseMatrix GetMatrix()
    {
        return new BaseMatrix(
            new[,]
            {
                { 4d, 2d, -1d },
                { 2d, 16d, 2d },
                { -1d, 2d, 4d }
            }
        );
    }
}