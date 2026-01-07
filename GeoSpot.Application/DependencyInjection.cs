using GeoSpot.Application.Services;
using GeoSpot.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<ISmsService, SmsService>();
        services.AddScoped<ICacheService, InMemoryCacheService>();
        services.AddScoped<IVerificationCodeGenerator, MockVerificationCodeGenerator>();
        
        return services;
    }
}