namespace GeoSpot.Contracts.Error;

[ExcludeFromCodeCoverage]
public class BadRequestResponseDto
{
    public required string Details { get; init; }
}