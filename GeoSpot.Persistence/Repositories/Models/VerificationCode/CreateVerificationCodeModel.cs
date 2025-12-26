namespace GeoSpot.Persistence.Repositories.Models.VerificationCode;

[ExcludeFromCodeCoverage]
public class CreateVerificationCodeModel
{
    public required string PhoneNumber { get; set; }
    
    public required string VerificationCode { get; set; }
}