namespace GeoSpot.Application.Dispatcher;

public interface IDispatcher
{
    Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request, CancellationToken ct = default) where TRequest : IRequest<TResponse>;
}