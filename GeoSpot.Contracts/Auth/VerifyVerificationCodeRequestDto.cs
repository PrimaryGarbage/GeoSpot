namespace GeoSpot.Contracts.Auth;

public record VerifyVerificationCodeRequestDto(string PhoneNumber, string VerificationCode);