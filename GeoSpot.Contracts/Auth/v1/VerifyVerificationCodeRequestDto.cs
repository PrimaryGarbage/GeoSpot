namespace GeoSpot.Contracts.Auth.v1;

public record VerifyVerificationCodeRequestDto(string PhoneNumber, string VerificationCode);