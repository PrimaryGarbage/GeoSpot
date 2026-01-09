using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GeoSpot.Persistence.Entities.EntityConfiguration;

internal class VerificationCodeEntityConfiguration : IEntityTypeConfiguration<VerificationCodeEntity>
{
    public void Configure(EntityTypeBuilder<VerificationCodeEntity> builder)
    {
        builder.ToTable(VerificationCodeEntity.TableName);
        builder.HasKey(x => x.VerificationCodeId);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.VerificationCode).HasMaxLength(6);
        
        builder.HasIndex(x => x.PhoneNumber);
    }
}