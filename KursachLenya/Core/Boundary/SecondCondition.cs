using KursachLenya.Core.GridComponents;
using KursachLenya.Core.Local;

namespace KursachLenya.Core.Boundary;

public record struct SecondCondition(int ElementIndex, Bound Bound);
public record struct SecondConditionValue(LocalVector Vector);