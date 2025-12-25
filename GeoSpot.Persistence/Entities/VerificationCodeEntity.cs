namespace GeoSpot.Persistence.Entities;

[ExcludeFromCodeCoverage]
internal class VerificationCodeEntity : IAuditEntity
{
    public const string TableName = "verification_codes";
    
    public Guid VerificationCodeId { get; set; }
    
    public required string PhoneNumber { get; set; }
    
    public required string VerificationCode { get; set; }
    
    public DateTime CreatedAt { get; set; }
    
    public DateTime UpdatedAt { get; set; }
    
    public int Attempts { get; set; }
}