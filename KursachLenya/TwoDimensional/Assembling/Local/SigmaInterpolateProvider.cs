using KursachLenya.Core.GridComponents;
using KursachLenya.TwoDimensional.Parameters;

namespace KursachLenya.TwoDimensional.Assembling.Local;

public class SigmaInterpolateProvider
{
    private readonly LocalBasisFunctionsProvider _localBasisFunctionsProvider;
    private readonly MaterialFactory _materialFactory;

    public SigmaInterpolateProvider(LocalBasisFunctionsProvider localBasisFunctionsProvider, MaterialFactory materialFactory)
    {
        _materialFactory = materialFactory;
        _localBasisFunctionsProvider = localBasisFunctionsProvider;
    }

    public Func<Node2D, double> GetSigmaInterpolate(Element element)
    {
        var sigmas = _materialFactory.GetById(element.MaterialId).Sigmas;
        var bilinearFunctions = _localBasisFunctionsProvider.GetBilinearFunctions(element);

        return p =>
            sigmas[0] * bilinearFunctions[0].Calculate(p) +
            sigmas[1] * bilinearFunctions[1].Calculate(p) +
            sigmas[2] * bilinearFunctions[2].Calculate(p) +
            sigmas[3] * bilinearFunctions[3].Calculate(p);
    }
}