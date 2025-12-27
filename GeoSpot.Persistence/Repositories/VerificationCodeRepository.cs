using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Mappers;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
internal class VerificationCodeRepository : BaseGeoSpotRepository, IVerificationCodeRepository
{
    public VerificationCodeRepository(GeoSpotDbContext dbContext) : base(dbContext)
    {}
    
    public async Task<VerificationCodeModel> CreateVerificationCodeAsync(CreateVerificationCodeModel createModel, CancellationToken ct = default)
    {
        VerificationCodeEntity entity = createModel.MapToEntity();
        
        DbContext.VerificationCodes.Add(entity);
        
        await DbContext.SaveChangesAsync(ct);
        
        return entity.MapToModel();
    }

    public async Task<VerificationCodeModel?> GetVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default)
    {
        VerificationCodeEntity? entity = await DbContext.VerificationCodes.FindAsync([verificationCodeId], cancellationToken: ct);
        
        return entity.MapToModelOrNull();
    }

    public async Task DeleteVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default)
    {
        VerificationCodeEntity? code = await DbContext.VerificationCodes.FindAsync([verificationCodeId], ct);
        if(code is null) return;
        
        DbContext.Entry(code).State = EntityState.Deleted;
        
        await DbContext.SaveChangesAsync(ct);
    }
}