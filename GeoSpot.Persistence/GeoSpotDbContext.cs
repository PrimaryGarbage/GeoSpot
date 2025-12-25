using GeoSpot.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence;

internal class GeoSpotDbContext : DbContext
{
    private const string DefaultSchema = "geospot";
    
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<BusinessProfileEntity> BusinessProfiles { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<DeviceTokenEntity> DeviceTokens { get; set; }
    public DbSet<SpotEntity> Spots { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema(DefaultSchema);
        
        modelBuilder.HasPostgresEnum<AccountType>();
        modelBuilder.HasPostgresEnum<Gender>();
        modelBuilder.HasPostgresEnum<SpotType>();
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GeoSpotDbContext).Assembly);
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e is { Entity: BaseAuditEntity, State: EntityState.Added or EntityState.Modified });
            
        foreach (var entry in entries)
        {
            var entity = (BaseAuditEntity)entry.Entity;
            if (entry.State == EntityState.Added) entity.CreatedAt = DateTime.UtcNow;
            
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }
}