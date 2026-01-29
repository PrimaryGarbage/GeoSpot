using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class DeleteSpotRequestValidator : AbstractValidator<DeleteSpotRequest>
{
    public DeleteSpotRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}