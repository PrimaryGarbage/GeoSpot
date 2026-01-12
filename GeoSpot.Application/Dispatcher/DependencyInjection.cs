using GeoSpot.Application.Dispatcher.HandlerBehaviors;
using GeoSpot.Common.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Application.Dispatcher;

public static class DependencyInjection
{
    public static IServiceCollection RegisterHandlers(this IServiceCollection services)
    {
        foreach(var handlerTypes in ResolveHandlerTypes())
            services.AddTransient(handlerTypes.HandlerInterfaceType, handlerTypes.HandlerType);
        
        services.AddTransient<IDispatcher, Dispatcher>();
        
        // Add handler behaviors here
        services.AddTransient(typeof(IHandlerBehavior<,>), typeof(ValidationHandlerBehavior<,>));
        
        return services;
    }
    
    private static IEnumerable<(Type HandlerInterfaceType, Type HandlerType)> ResolveHandlerTypes()
    {
        var assembly = typeof(IDispatcher).Assembly;
        var requestGenericInterfaceType = typeof(IRequest<>);
        var handlerGenericInterfaceType = typeof(IRequestHandler<,>);
        
        var requestTypes = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false })
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestGenericInterfaceType));
        
        foreach (var requestType in requestTypes)
        {
            var responseType = requestType.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == requestGenericInterfaceType)
                .GenericTypeArguments[0];
            var handlerInterfaceType = handlerGenericInterfaceType.MakeGenericType(requestType, responseType);
            var handlerTypes = assembly.GetTypes().Where(t => 
                handlerInterfaceType.IsAssignableFrom(t) && t is { IsClass: true, IsAbstract: false }).ToList();
            
            if (handlerTypes.Count > 1)
                throw new InitializationException($"More than one handler discovered for the request type. Request type : ${requestType.FullName}");
            if (handlerTypes.Count == 0)
                continue;
            
            var handlerType = handlerTypes[0];
            
            yield return (handlerInterfaceType, handlerType);
        }
    }
}