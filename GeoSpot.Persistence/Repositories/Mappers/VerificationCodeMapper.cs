using GeoSpot.Persistence.Entities;
using GeoSpot.Persistence.Repositories.Models.VerificationCode;

namespace GeoSpot.Persistence.Repositories.Mappers;

[ExcludeFromCodeCoverage]
internal static class VerificationCodeMapper
{
    public static VerificationCodeEntity MapToEntity(this CreateVerificationCodeModel input)
    {
        return new VerificationCodeEntity
        {
            PhoneNumber = input.PhoneNumber,
            VerificationCode = input.VerificationCode,
        };
    }

    public static VerificationCodeModel MapToModel(this VerificationCodeEntity input)
    {
        return new VerificationCodeModel
        {
            VerificationCodeId = input.VerificationCodeId,
            Attempts = input.Attempts,
            PhoneNumber = input.PhoneNumber,
            VerificationCode = input.VerificationCode,
            CreatedAt = input.CreatedAt
        };
    }

    public static VerificationCodeModel? MapToModelOrNull(this VerificationCodeEntity? input)
    {
        return input?.MapToModel();
    }
}