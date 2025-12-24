namespace GeoSpot.Persistence.Entities;

public class VerificationCodeEntity : BaseAuditExtendedEntity
{
    public Guid Id { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public required string VerificationCode { get; set; }
    
    public int Attempts { get; set; }
}