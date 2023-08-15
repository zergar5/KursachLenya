namespace KursachLenya.Core.GridComponents;

public class Element
{
    public int[] NodesIndexes { get; }
    public int MaterialId { get; }
    public double Length { get; }
    public double Height { get; }

    public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)NodesIndexes).GetEnumerator();

    public Element(int[] nodesIndexes, double length, double height, int materialId)
    {
        NodesIndexes = nodesIndexes;
        Length = length;
        Height = height;
        MaterialId = materialId;
    }

    public (int[], double) GetBoundNodeIndexes(Bound bound) =>
        bound switch
        {
            Bound.Lower =>
            (new[]
            {
                NodesIndexes[0],
                NodesIndexes[1],
                NodesIndexes[2]
            },
                Length
            ),
            Bound.Left =>
            (new[]
            {
                NodesIndexes[0],
                NodesIndexes[3],
                NodesIndexes[6]
            },
                Height
            ),
            Bound.Right =>
            (new[]
            {
                NodesIndexes[2],
                NodesIndexes[5],
                NodesIndexes[8]
            },
                Height
            ),
            Bound.Upper =>
            (new[]
            {
                NodesIndexes[6],
                NodesIndexes[7],
                NodesIndexes[8]
            },
                Length
            ),
            _ => throw new ArgumentOutOfRangeException()
        };
}