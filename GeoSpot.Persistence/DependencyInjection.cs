using GeoSpot.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using GeoSpot.Common.Enums;
using GeoSpot.Persistence.Repositories;
using GeoSpot.Persistence.Repositories.Interfaces;
using static GeoSpot.Common.Constants.ConfigurationConstants;

namespace GeoSpot.Persistence;

[ExcludeFromCodeCoverage]
public static class DependencyInjection
{
    public static IServiceCollection AddGeoSpotPersistenceModule(this IServiceCollection services, IConfiguration configuration)
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
        
        services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            
        return services;
    }
    
    public static void PrepareDatabase(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<GeoSpotDbContext>();
        db.Database.Migrate();
    }
}