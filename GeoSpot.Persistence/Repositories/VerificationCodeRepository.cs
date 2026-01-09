using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Interfaces;
using GeoSpot.Persistence.Repositories.Mappers;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;
using Microsoft.EntityFrameworkCore;

namespace GeoSpot.Persistence.Repositories;

[ExcludeFromCodeCoverage]
internal class VerificationCodeRepository : BaseGeoSpotRepository, IVerificationCodeRepository
{
    public VerificationCodeRepository(GeoSpotDbContext dbContext, IUnitOfWork unitOfWork) : base(dbContext, unitOfWork)
    {}
    
    public async Task<VerificationCodeModel> CreateVerificationCodeAsync(CreateVerificationCodeModel createModel, CancellationToken ct = default)
    {
        VerificationCodeEntity entity = createModel.MapToEntity();
        
        DbContext.VerificationCodes.Add(entity);
        
        await SaveChangesAsync(ct);
        
        return entity.MapToModel();
    }

    public async Task<VerificationCodeModel?> GetVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default)
    {
        VerificationCodeEntity? entity = await DbContext.VerificationCodes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.VerificationCodeId == verificationCodeId, ct);
        
        return entity.MapToModelOrNull();
    }

    public async Task<VerificationCodeModel?> GetVerificationCodeAsync(string phoneNumber, string verificationCode, CancellationToken ct = default)
    {
        VerificationCodeEntity? entity = await DbContext.VerificationCodes.AsNoTracking()
            .FirstOrDefaultAsync(x => x.PhoneNumber == phoneNumber && x.VerificationCode == verificationCode, ct);

        return entity.MapToModelOrNull();
    }

    public async Task DeleteVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default)
    {
        VerificationCodeEntity? code = await DbContext.VerificationCodes.FindAsync([verificationCodeId], ct);
        if(code is null) return;
        
        DbContext.Entry(code).State = EntityState.Deleted;
        
        await SaveChangesAsync(ct);
    }

    public async Task DeleteAllUserVerificationCodesAsync(string phoneNumber, CancellationToken ct = default)
    {
        var codesToDelete = await DbContext.VerificationCodes.Where(x => x.PhoneNumber == phoneNumber).ToListAsync(ct);
        
        DbContext.VerificationCodes.RemoveRange(codesToDelete);
        
        await SaveChangesAsync(ct);
    }
}