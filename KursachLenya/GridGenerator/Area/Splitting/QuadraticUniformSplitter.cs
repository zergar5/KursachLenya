using KursachLenya.GridGenerator.Area.Core;

namespace KursachLenya.GridGenerator.Area.Splitting;

public readonly record struct QuadraticUniformSplitter(int Steps) : IIntervalSplitter
{
    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length / (2 * Steps);

        for (var stepNumber = 0; stepNumber <= 2 * Steps; stepNumber++)
        {
            var value = interval.Begin + stepNumber * step;

            yield return value;
        }
    }
}