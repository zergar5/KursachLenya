using KursachLenya.Core.Global;
using KursachLenya.Core.Local;

namespace KursachLenya.FEM.Assembling;

public interface IInserter<in TMatrix>
{
    public void InsertMatrix(TMatrix globalMatrix, LocalMatrix localMatrix);
    public void InsertVector(GlobalVector vector, LocalVector localVector);
}