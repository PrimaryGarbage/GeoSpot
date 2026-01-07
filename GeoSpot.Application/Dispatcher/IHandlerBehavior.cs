namespace GeoSpot.Application.Dispatcher;

internal delegate Task<TResponse> RequestHandlerDelegate<TResponse>(CancellationToken ct = default);

internal interface IHandlerBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default);
}