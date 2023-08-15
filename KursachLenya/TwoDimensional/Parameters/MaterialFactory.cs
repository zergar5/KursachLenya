using KursachLenya.Core.GridComponents;

namespace KursachLenya.TwoDimensional.Parameters;

public class MaterialFactory
{
    private readonly Dictionary<int, double> _lambdas;
    private readonly Dictionary<int, double[]> _sigmas;

    public MaterialFactory(IEnumerable<double> lambdas, IEnumerable<double[]> sigmas)
    {
        _lambdas = lambdas.Select((value, index) => new KeyValuePair<int, double>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
        _sigmas = sigmas.Select((value, index) => new KeyValuePair<int, double[]>(index, value))
            .ToDictionary(index => index.Key, value => value.Value);
    }

    public Material GetById(int id)
    {
        return new Material(
            _lambdas[id],
            _sigmas[id]
        );
    }
}