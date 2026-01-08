using GeoSpot.Persistence.Repositories.Models.VerificationCode;

namespace GeoSpot.Persistence.Repositories.Interfaces;

public interface IVerificationCodeRepository
{
    Task<VerificationCodeModel> CreateVerificationCodeAsync(CreateVerificationCodeModel createModel, CancellationToken ct = default);
    
    Task<VerificationCodeModel?> GetVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default);
    
    Task<VerificationCodeModel?> GetVerificationCodeAsync(string verificationCode, CancellationToken ct = default);
    
    Task DeleteVerificationCodeAsync(Guid verificationCodeId, CancellationToken ct = default);
}