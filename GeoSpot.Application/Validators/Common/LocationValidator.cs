using FluentValidation;

namespace GeoSpot.Application.Validators.Common;

public class Location
{
    public double Latitude { get; init; }

    public double Longitude { get; init; }

    public Location(double latitude, double longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}

public class LocationValidator : AbstractValidator<Location>
{
    public LocationValidator()
    {
        RuleFor(x => Math.Abs(x.Latitude)).LessThanOrEqualTo(90.0);
        RuleFor(x => Math.Abs(x.Longitude)).LessThanOrEqualTo(180.0);
    }
}