namespace GeoSpot.Application.Dispatcher;

public interface IDispatcher
{
    Task<TResponse> Dispatch<TRequest, TResponse>(TRequest request, CancellationToken ct = default) where TRequest : IRequest<TResponse>;
}