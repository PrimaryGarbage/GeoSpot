using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class RemoveSpotReactionRequestValidator : AbstractValidator<RemoveSpotReactionRequest>
{
    public RemoveSpotReactionRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}