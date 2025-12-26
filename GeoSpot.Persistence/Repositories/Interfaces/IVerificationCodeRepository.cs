using GeoSpot.Persistence.Repositories.Models.VerificationCode;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IVerificationCodeRepository
{
    Task<VerificationCodeModel> CreateVerificationCodeAsync(CreateVerificationCodeModel createModel, CancellationToken ct);
    
    Task<VerificationCodeModel> GetVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct);
}