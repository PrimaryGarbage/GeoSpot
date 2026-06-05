using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using GeoSpot.Common.Enums;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace GeoSpot.Persistence;

public class GeoSpotDbContext : DbContext
{
    public const string DefaultSchema = "geospot";
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<BusinessProfileEntity> BusinessProfiles { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<DeviceTokenEntity> DeviceTokens { get; set; }
    public DbSet<SpotEntity> Spots { get; set; }
    public DbSet<VerificationCodeEntity> VerificationCodes { get; set; }
    public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }
    public DbSet<UserSpotViewEntity> UserSpotViews { get; set; }
    public DbSet<SpotReactionEntity> SpotReactions { get; set; }
    public DbSet<ReactionTypeEntity> ReactionTypes { get; set; }
    
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
        IEnumerable<EntityEntry> entries = ChangeTracker.Entries().Where(e =>
            e is { Entity: IAuditEntity, State: EntityState.Added or EntityState.Modified });
            
        foreach (var entry in entries)
        {
            if (entry.Entity is IAuditEntity auditEntity)
                UpdateTimestamps(auditEntity, entry.State == EntityState.Added);

            if (entry.Entity is IPositionedEntity positionedEntity)
                UpdatePosition(positionedEntity);
        }

        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        IEnumerable<EntityEntry> entries = ChangeTracker.Entries()
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