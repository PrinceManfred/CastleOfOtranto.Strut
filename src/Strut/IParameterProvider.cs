namespace CastleOfOtranto.Strut;

public interface IParameterProvider
{
    public IEnumerable<ParameterDescription> GetParameters();
}

