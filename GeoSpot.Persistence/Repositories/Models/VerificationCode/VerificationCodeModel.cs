namespace GeoSpot.Persistence.Repositories.Models.VerificationCode;

[ExcludeFromCodeCoverage]
public class VerificationCodeModel
{
    public Guid VerificationCodeId { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public required string VerificationCode { get; set; }
    
    public int Attempts { get; set; }
    
    public DateTime CreatedAt { get; set; }
}