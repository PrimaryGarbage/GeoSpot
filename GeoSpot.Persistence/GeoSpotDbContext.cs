using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence;

[ExcludeFromCodeCoverage]
internal class GeoSpotDbContext : DbContext
{
    public const string DefaultSchema = "geospot";
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<BusinessProfileEntity> BusinessProfiles { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<DeviceTokenEntity> DeviceTokens { get; set; }
    public DbSet<SpotEntity> Spots { get; set; }
    
    public GeoSpotDbContext(DbContextOptions<GeoSpotDbContext> options): base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.HasPostgresEnum<AccountType>(DefaultSchema);
        modelBuilder.HasPostgresEnum<Gender>(DefaultSchema);
        modelBuilder.HasPostgresEnum<SpotType>(DefaultSchema);
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoSpotDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e is { Entity: IAuditEntity, State: EntityState.Added or EntityState.Modified });
            
        foreach (var entry in entries)
        {
            var entity = (IAuditEntity)entry.Entity;
            if (entry.State == EntityState.Added) entity.CreatedAt = DateTime.UtcNow;
            
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State is EntityState.Added or EntityState.Modified);
        
        foreach (var entry in entries)
        {
            if (entry.Entity is IAuditEntity auditEntity)
                UpdateTimestamps(auditEntity, entry.State == EntityState.Added);

            if (entry.Entity is IPositionedEntity positionedEntity)
                UpdatePosition(positionedEntity);
        }

        return base.SaveChangesAsync(ct);
    }

    private static void UpdateTimestamps(IAuditEntity entity, bool added)
    {
        if (added) entity.CreatedAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
    }

    private static void UpdatePosition(IPositionedEntity entity)
    {
        if (entity.Latitude != 0 && entity.Longitude != 0)
        {
            entity.Position = new NetTopologySuite.Geometries.Point(entity.Longitude, entity.Latitude) { SRID = 4326 };
        }
    }
}