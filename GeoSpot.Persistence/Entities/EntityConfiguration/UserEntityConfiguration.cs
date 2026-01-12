using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
{
    private const string PhoneNumberIndexName = "idx_user_phone_number";
    
    public void Configure(EntityTypeBuilder<UserEntity> builder)
    {
        builder.ToTable(UserEntity.TableName);
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(x => x.Email).HasMaxLength(256);
        builder.Property(x => x.PasswordHash).HasMaxLength(256);
        builder.Property(x => x.PasswordSalt).HasMaxLength(32);
        builder.Property(x => x.DetectionRadius).HasDefaultValue(500);
        builder.Property(x => x.LastLatitude).HasPrecision(10, 8);
        builder.Property(x => x.LastLongitude).HasPrecision(11, 8);
        builder.Property(x => x.DisplayName).HasMaxLength(100);
        builder.Property(x => x.AvatarUrl).HasMaxLength(500);
        
        builder.HasMany(x => x.Categories)
            .WithMany()
            .UsingEntity<UserCategoryEntity>()
            .ToTable(UserCategoryEntity.TableName)
            .HasKey(x => new { x.UserId, x.CategoryId });
        builder.HasMany(x => x.UserSpotViews)
            .WithOne(x => x.User);
        builder.HasMany(x => x.BusinessProfiles)
            .WithOne(x => x.Creator);
        builder.HasMany(x => x.CreatedSpots)
            .WithOne(x => x.Creator);
        builder.HasMany(x => x.Comments)
            .WithOne(x => x.Creator);
        builder.HasMany(x => x.Reactions)
            .WithOne(x => x.Creator);
        builder.HasMany(x => x.DeviceTokens)
            .WithOne(x => x.User);
        
        builder.HasIndex(x => x.PhoneNumber).IsUnique().HasDatabaseName(PhoneNumberIndexName);
    }
}