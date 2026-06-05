using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class AddSpotReactionRequestValidator : AbstractValidator<AddSpotReactionRequest>
{
    public AddSpotReactionRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
        RuleFor(x => x.Dto.ReactionTypeId).NotEmpty();
    }
}