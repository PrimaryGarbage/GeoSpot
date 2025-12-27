using System.Diagnostics.CodeAnalysis;

namespace GeoSpot.Contracts.Auth;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeRequestDto
{
    public Guid VerificationCodeId { get; set; }
    
    public required string VerificationCode { get; set; }
}