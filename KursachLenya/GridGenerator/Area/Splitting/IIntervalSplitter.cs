using KursachLenya.GridGenerator.Area.Core;

namespace KursachLenya.GridGenerator.Area.Splitting;

public interface IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval);
    public int Steps { get; }
}