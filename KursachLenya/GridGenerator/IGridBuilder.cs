using KursachLenya.Core;

namespace KursachLenya.GridGenerator;

public interface IGridBuilder<TPoint>
{
    public Grid<TPoint> Build();
}