using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;
using GeoSpot.Application.Validators.Common;
using GeoSpot.Common.ConfigurationSections;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Validators.Spot;

public class CreateSpotRequestValidator : AbstractValidator<CreateSpotRequest>
{
    public CreateSpotRequestValidator(IOptions<GeolocationConfigurationSection> options, IValidator<Location> locationValidator)
    {
        RuleFor(x => x.Dto.Title).NotEmpty();
        RuleFor(x => x.Dto.BusinessProfileId!.Value)
            .NotEmpty()
            .When(x => x.Dto.BusinessProfileId is not null);
        RuleFor(x => x.Dto.SpotType).IsInEnum();
        RuleFor(x => x.Dto.Latitude).NotEmpty();
        RuleFor(x => new Location(x.Dto.Latitude, x.Dto.Longitude)).SetValidator(locationValidator);
        RuleFor(x => x.Dto.Radius)
            .GreaterThan(0)
            .LessThanOrEqualTo(options.Value.MaxSpotRadius);
        RuleFor(x => x.Dto.StartsAt).GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => (x.Dto.EndsAt - x.Dto.StartsAt).TotalSeconds).LessThanOrEqualTo(options.Value.MaxSpotLifetimeSeconds);
    }
}