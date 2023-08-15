using KursachLenya.Core;
using KursachLenya.Core.GridComponents;
using KursachLenya.GridGenerator.Area.Splitting;

namespace KursachLenya.GridGenerator;

public class GridBuilder2D : IGridBuilder<Node2D>
{
    private AxisSplitParameter? _rAxisSplitParameter;
    private AxisSplitParameter? _zAxisSplitParameter;
    private int[]? _materialsId;

    private int GetTotalRElements => _rAxisSplitParameter.Splitters.Sum(r => r.Steps);
    private int GetTotalZElements => _zAxisSplitParameter.Splitters.Sum(z => z.Steps);

    public GridBuilder2D SetRAxis(AxisSplitParameter splitParameter)
    {
        _rAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder2D SetZAxis(AxisSplitParameter splitParameter)
    {
        _zAxisSplitParameter = splitParameter;
        return this;
    }

    public GridBuilder2D SetMaterials(int[] materialsId)
    {
        _materialsId = materialsId;
        return this;
    }

    public Grid<Node2D> Build()
    {
        if (_rAxisSplitParameter == null || _zAxisSplitParameter == null)
            throw new ArgumentNullException();

        var totalRElements = GetTotalRElements;

        var totalNodes = GetTotalNodes();
        var totalElements = GetTotalElements();

        var nodes = new Node2D[totalNodes];
        var elements = new Element[totalElements];

        _materialsId ??= new int[totalElements];

        var i = 0;

        foreach (var (zSection, zSplitter) in _zAxisSplitParameter.SectionWithParameter)
        {
            var zValues = zSplitter.EnumerateValues(zSection);
            if (i > 1) zValues = zValues.Skip(2);

            foreach (var z in zValues)
            {
                var j = 0;

                foreach (var (rSection, rSplitter) in _rAxisSplitParameter.SectionWithParameter)
                {
                    var rValues = rSplitter.EnumerateValues(rSection);
                    if (j > 1) rValues = rValues.Skip(2);

                    foreach (var r in rValues)
                    {
                        var nodeIndex = j + i * (2 * totalRElements + 1);

                        nodes[nodeIndex] = new Node2D(r, z);

                        if (i > 1 && j > 1 && (i % 2 == 0 && j % 2 == 0))
                        {
                            var elementIndex = (j / 2 - 1) + (i / 2 - 1) * totalRElements;
                            var nodesIndexes = GetCurrentElementIndexes(i - 2, j - 2);

                            elements[elementIndex] = new Element(
                                nodesIndexes,
                                nodes[nodesIndexes[2]].R - nodes[nodesIndexes[0]].R,
                                nodes[nodesIndexes[6]].Z - nodes[nodesIndexes[0]].Z,
                                _materialsId[elementIndex]
                                );
                        }

                        j++;
                    }
                }

                i++;
            }
        }

        return new Grid<Node2D>(nodes, elements);
    }

    private int GetTotalNodes()
    {
        return (2 * GetTotalRElements + 1) * (2 * GetTotalZElements + 1);
    }

    private int GetTotalElements()
    {
        return GetTotalRElements * GetTotalZElements;
    }

    private int[] GetCurrentElementIndexes(int z, int r)
    {
        var totalRElements = GetTotalRElements;

        var indexes = new int[9];

        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                indexes[j + i * 3] = r + j + (z + i) * (2 * totalRElements + 1);
            }
        }

        return indexes;
    }
}