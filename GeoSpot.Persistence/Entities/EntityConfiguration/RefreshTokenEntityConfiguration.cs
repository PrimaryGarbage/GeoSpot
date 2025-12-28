using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

[ExcludeFromCodeCoverage]
internal class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<RefreshTokenEntity>
{
    private const string TokenHashIndexName = "idx_refresh_token_token_hash";
    private const string UserIdIndexName = "idx_refresh_token_user_id";

    public void Configure(EntityTypeBuilder<RefreshTokenEntity> builder)
    {
        builder.ToTable(RefreshTokenEntity.TableName);
        builder.HasKey(x => x.RefreshTokenId);
        builder.Property(x => x.TokenHash).HasMaxLength(44);
        
        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId);
        
        builder.HasIndex(x => x.TokenHash).HasDatabaseName(TokenHashIndexName);
        builder.HasIndex(x => x.UserId).HasDatabaseName(UserIdIndexName);
    }
}