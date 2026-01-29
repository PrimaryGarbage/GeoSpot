using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class AddSpotViewRequestValidator : AbstractValidator<AddSpotViewRequest>
{
    public AddSpotViewRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}