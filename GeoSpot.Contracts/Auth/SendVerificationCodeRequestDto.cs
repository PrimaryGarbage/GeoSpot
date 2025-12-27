using System.Diagnostics.CodeAnalysis;

namespace GeoSpot.Contracts.Auth;

public record SendVerificationCodeRequestDto(string PhoneNumber);