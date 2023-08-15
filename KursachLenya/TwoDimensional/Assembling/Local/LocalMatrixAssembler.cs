using KursachLenya.Core;
using KursachLenya.Core.Base;
using KursachLenya.Core.GridComponents;
using KursachLenya.FEM.Assembling;
using KursachLenya.FEM.Assembling.Local;
using KursachLenya.TwoDimensional.Parameters;

namespace KursachLenya.TwoDimensional.Assembling.Local;

public class LocalMatrixAssembler : ILocalMatrixAssembler
{
    private readonly Grid<Node2D> _grid;
    private readonly MaterialFactory _materialFactory;
    private readonly ITemplateMatrixProvider _stiffnessMatrixTemplateProvider;
    private readonly ITemplateMatrixProvider _stiffnessMatrixRTemplateProvider;
    private readonly ITemplateMatrixProvider _massMatrixTemplateProvider;
    private readonly ITemplateMatrixProvider _massMatrixRTemplateProvider;

    public LocalMatrixAssembler
    (
        Grid<Node2D> grid,
        MaterialFactory materialFactory,
        ITemplateMatrixProvider stiffnessMatrixTemplateProvider,
        ITemplateMatrixProvider stiffnessMatrixRTemplateProvider,
        ITemplateMatrixProvider massMatrixTemplateProvider,
        ITemplateMatrixProvider massMatrixRTemplateProvider
    )
    {
        _grid = grid;
        _materialFactory = materialFactory;
        _stiffnessMatrixTemplateProvider = stiffnessMatrixTemplateProvider;
        _stiffnessMatrixRTemplateProvider = stiffnessMatrixRTemplateProvider;
        _massMatrixTemplateProvider = massMatrixTemplateProvider;
        _massMatrixRTemplateProvider = massMatrixRTemplateProvider;
    }

    public BaseMatrix AssembleStiffnessMatrix(Element element)
    {
        var stiffness = new BaseMatrix(element.NodesIndexes.Length);

        var lambda = _materialFactory.GetById(element.MaterialId).Lambda;

        var stiffnessR = AssembleStiffnessR(element);
        var stiffnessZ = AssembleStiffnessZ(element);

        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                stiffness[i, j] = lambda *
                                  (stiffnessR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)] +
                                   massR[GetMuIndex(i), GetMuIndex(j)] * stiffnessZ[GetNuIndex(i), GetNuIndex(j)]);
                stiffness[j, i] = stiffness[i, j];
            }
        }

        return stiffness;
    }

    public BaseMatrix AssembleMassMatrix(Element element)
    {
        var mass = new BaseMatrix(element.NodesIndexes.Length);

        var massR = AssembleMassR(element);
        var massZ = AssembleMassZ(element);

        for (var i = 0; i < element.NodesIndexes.Length; i++)
        {
            for (var j = 0; j <= i; j++)
            {
                mass[i, j] = massR[GetMuIndex(i), GetMuIndex(j)] * massZ[GetNuIndex(i), GetNuIndex(j)];
                mass[j, i] = mass[i, j];
            }
        }

        return mass;
    }

    private BaseMatrix AssembleStiffnessR(Element element)
    {
        var stiffnessR = _stiffnessMatrixRTemplateProvider.GetMatrix();
        stiffnessR = 1d / 6d * stiffnessR;

        var stiffnessZ = _stiffnessMatrixTemplateProvider.GetMatrix();
        stiffnessZ = _grid.Nodes[element.NodesIndexes[0]].R / (3d * element.Height) * stiffnessZ;

        BaseMatrix.Sum(stiffnessR, stiffnessZ);

        return stiffnessR;
    }

    private BaseMatrix AssembleStiffnessZ(Element element)
    {
        var stiffnessZ = _stiffnessMatrixTemplateProvider.GetMatrix();
        stiffnessZ = 1d / (3d * element.Height) * stiffnessZ;

        return stiffnessZ;
    }

    private BaseMatrix AssembleMassR(Element element)
    {
        var massR = _massMatrixRTemplateProvider.GetMatrix();
        massR = Math.Pow(element.Length, 2) / 60d * massR;

        var massZ = _massMatrixTemplateProvider.GetMatrix();
        massZ = element.Length * _grid.Nodes[element.NodesIndexes[0]].R / 30d * massZ;

        massR = BaseMatrix.Sum(massR, massZ);

        return massR;
    }

    private BaseMatrix AssembleMassZ(Element element)
    {
        var massZ = _massMatrixTemplateProvider.GetMatrix();
        massZ = element.Height / 30d * massZ;

        return massZ;
    }

    private int GetMuIndex(int i)
    {
        return i % 3;
    }

    private int GetNuIndex(int i)
    {
        return i / 3;
    }
}