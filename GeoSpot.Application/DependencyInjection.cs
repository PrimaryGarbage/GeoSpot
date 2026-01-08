using GeoSpot.Application.Services;
using GeoSpot.Application.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GeoSpot.Application;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddTransient<IJwtTokenService, JwtTokenService>();
        services.AddTransient<ISmsService, SmsService>();
        services.AddTransient<ICacheService, InMemoryCacheService>();
        services.AddTransient<IVerificationCodeGenerator, MockVerificationCodeGenerator>();
        services.AddScoped<IUserClaimsAccessor, UserClaimsAccessor>();
        
        return services;
    }
}