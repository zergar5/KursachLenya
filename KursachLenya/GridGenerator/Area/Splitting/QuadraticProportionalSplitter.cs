using KursachLenya.GridGenerator.Area.Core;

namespace KursachLenya.GridGenerator.Area.Splitting;

public class QuadraticProportionalSplitter : IIntervalSplitter
{
    public int Steps { get; }
    public double DischargeRatio { get; }

    private readonly double _lengthCoefficient;

    public QuadraticProportionalSplitter(int steps, double dischargeRatio)
    {
        if (Math.Abs(DischargeRatio - 1d) < 1e-16)
            throw new NotSupportedException();

        Steps = steps;
        DischargeRatio = dischargeRatio;
        _lengthCoefficient = (DischargeRatio - 1d) / (Math.Pow(DischargeRatio, Steps) - 1d);
    }

    public IEnumerable<double> EnumerateValues(Interval interval)
    {
        var step = interval.Length * _lengthCoefficient;

        var leftValue = interval.Begin + step * (Math.Pow(DischargeRatio, 0) - 1d) / (DischargeRatio - 1d);
        yield return leftValue;

        for (var stepNumber = 1; stepNumber <= Steps; stepNumber++)
        {
            var rightValue = interval.Begin + step * (Math.Pow(DischargeRatio, stepNumber) - 1d) / (DischargeRatio - 1d);
            yield return (rightValue + leftValue) / 2;
            leftValue = rightValue;
            yield return rightValue;
        }
    }
}