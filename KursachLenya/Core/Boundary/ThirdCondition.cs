using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;

namespace KursachLenya.Core.Boundary;

public record struct ThirdCondition(int ElementIndex, Bound Bound, double Beta);
public readonly record struct ThirdConditionValue(LocalMatrix Matrix, LocalVector Vector);