using GeoSpot.Common.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Application.Dispatcher;

[ExcludeFromCodeCoverage]
internal class Dispatcher : IDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    
    public Dispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public Task<TResponse> DispatchAsync<TRequest, TResponse>(TRequest request, CancellationToken ct = default) where TRequest : IRequest<TResponse>
    {
        var handler = _serviceProvider.GetRequiredService<IRequestHandler<TRequest, TResponse>>();
        var behaviors = _serviceProvider.GetServices<IHandlerBehavior<TRequest, TResponse>>();
        
        RequestHandlerDelegate<TResponse> pipeline = (cancellationToken) => handler.Handle(request, cancellationToken);
        foreach(var behavior in behaviors.Reverse())
        {
            var next = pipeline;
            pipeline = (cancellationToken) => behavior.Handle(request, next, cancellationToken);
        }
        
        return pipeline.Invoke(ct);
    }
}