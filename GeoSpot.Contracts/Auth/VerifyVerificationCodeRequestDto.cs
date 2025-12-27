namespace GeoSpot.Contracts.Auth;

public record VerifyVerificationCodeRequestDto(Guid VerificationCodeId, string VerificationCode);