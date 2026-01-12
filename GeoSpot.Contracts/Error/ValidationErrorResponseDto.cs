namespace GeoSpot.Contracts.Error;

public class ValidationErrorResponseDto
{
    public required IEnumerable<ValidationError> Errors { get; init; }
}

public class ValidationError(string field, string error)
{
    public string Field { get; init; } = field;
    
    public string Error { get; init; } = error;
}