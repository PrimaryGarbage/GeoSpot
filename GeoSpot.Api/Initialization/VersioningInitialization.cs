using Asp.Versioning;

namespace GeoSpot.Api.Initialization;

[ExcludeFromCodeCoverage]
internal static class VersioningInitialization
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = false;
            options.DefaultApiVersion = new ApiVersion(1.0);
            options.ReportApiVersions = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                //new HeaderApiVersionReader("X-Version"),
                new UrlSegmentApiVersionReader()
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