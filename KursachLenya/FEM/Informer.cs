using KursachLenya.Core.GridComponents;

namespace KursachLenya.FEM;

public class Informer
{
    public static void GetInfo(int iteration, double residual)
    {
        Console.Write($"Iteration: {iteration}, residual: {residual:E14}                                   \r");
    }

    public static void WriteSolution(Node2D point, double time, double value)
    {
        Console.WriteLine($"({point.R},{point.Z}) {time} {value:E14}");
    }

    public static void WriteAreaInfo()
    {
        Console.WriteLine("Point not in area or time interval");
    }
}