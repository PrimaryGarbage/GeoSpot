namespace GeoSpot.Persistence.Entities;

internal class VerificationCodeEntity : BaseAuditEntity
{
    public const string TableName = "verification_codes";
    
    public Guid VerificationCodeId { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public required string VerificationCode { get; set; }
    
    public int Attempts { get; set; }
}