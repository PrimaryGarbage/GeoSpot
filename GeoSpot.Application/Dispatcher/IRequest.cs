namespace GeoSpot.Application.Dispatcher;

public interface IRequest<TResponse> {}

public class Empty
{
    public static Empty Value { get; } = new();
}
