using Asp.Versioning;
using GeoSpot.Api.Constants;

namespace GeoSpot.Api.Initialization;

[ExcludeFromCodeCoverage]
internal static class VersioningInitialization
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new HeaderApiVersionReader(VersioningConstants.VersioningHeaderName)
                //new UrlSegmentApiVersionReader()
                );
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });
        
        return services;
    }
}