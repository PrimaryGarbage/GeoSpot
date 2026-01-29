using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;
using GeoSpot.Application.Validators.Common;
using GeoSpot.Common.ConfigurationSections;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Validators.Spot;

public class UpdateSpotRequestValidator : AbstractValidator<UpdateSpotRequest>
{
    public UpdateSpotRequestValidator(IOptions<GeolocationConfigurationSection> options, IValidator<Location> locationValidator)
    {
        RuleFor(x => x.SpotId).NotEmpty();
        RuleFor(x => x.Dto.Title).NotEmpty();
        RuleFor(x => x.Dto.SpotType).IsInEnum();
        RuleFor(x => new Location(x.Dto.Latitude, x.Dto.Longitude)).SetValidator(locationValidator);
        RuleFor(x => x.Dto.Radius)
            .GreaterThan(0)
            .LessThanOrEqualTo(options.Value.MaxSpotRadius);
    }
}