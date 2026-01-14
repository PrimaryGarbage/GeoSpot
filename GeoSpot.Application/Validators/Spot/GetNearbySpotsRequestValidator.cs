using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;
using GeoSpot.Common.ConfigurationSections;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Validators.Spot;

public class GetNearbySpotsRequestValidator : AbstractValidator<SearchNearbySpotsRequest>
{
    public GetNearbySpotsRequestValidator(IOptions<GeolocationConfigurationSection> options)
    {
        RuleFor(x => Math.Abs(x.Dto.Latitude)).LessThanOrEqualTo(90.0);
        RuleFor(x => Math.Abs(x.Dto.Longitude)).LessThanOrEqualTo(190.0);
        RuleFor(x => x.Dto.Radius)
            .GreaterThan(0)
            .LessThanOrEqualTo(options.Value.MaxSearchDistance);
    }
}