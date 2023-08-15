using KursachLenya.Core.GridComponents;

namespace KursachLenya.Core.Boundary;

public record struct FirstCondition(int ElementIndex, Bound Bound);
public record struct FirstConditionValue(int[] NodesIndexes, double[] Values);