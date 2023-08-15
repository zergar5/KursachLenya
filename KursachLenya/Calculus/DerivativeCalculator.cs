using KursachLenya.Core.GridComponents;
using KursachLenya.TwoDimensional.Assembling.Local;

namespace KursachLenya.Calculus;

public class DerivativeCalculator
{
    private const double Delta = 1.0e-3;

    public double Calculate(Func<Node2D, double, double> function, Node2D point, double time, char variableChar)
    {
        double result;
        if (variableChar == 'r')
        {
            result = function(point with { R = point.R + Delta }, time) -
                     function(point with { R = point.R - Delta }, time);
        }
        else
        {
            result = function(point with { Z = point.Z + Delta }, time) -
                     function(point with { Z = point.Z - Delta }, time);
        }
        return result / (2.0 * Delta);
    }
}