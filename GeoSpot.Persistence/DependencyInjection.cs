using GeoSpot.Common.Exceptions;
using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using static GeoSpot.Common.Constants.ConfigurationConstants;

namespace GeoSpot.Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddGeospotDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString(SqlDatabaseConnectionStringName)
            ?? throw new InitializationException("Failed to find database connection string");
    
        services.AddDbContext<GeoSpotDbContext>(options => 
        options.UseNpgsql(connectionString,
            o =>
            {
                o.UseNetTopologySuite(geographyAsDefault: true);
                o.MigrationsHistoryTable("__EFMigrationHistory", GeoSpotDbContext.DefaultSchema);
                o.MapEnum<AccountType>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<Gender>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<SpotType>(schemaName: GeoSpotDbContext.DefaultSchema);
                o.MapEnum<Platform>(schemaName: GeoSpotDbContext.DefaultSchema);
            })
            .UseSnakeCaseNamingConvention()
        );
            
        return services;
    }
    
    public static void PrepareDatabase(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoSpotDbContext>();
        db.Database.Migrate();
    }
}