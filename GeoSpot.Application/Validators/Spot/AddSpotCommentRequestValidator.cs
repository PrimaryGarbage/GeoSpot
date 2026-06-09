using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class AddSpotCommentRequestValidator : AbstractValidator<AddSpotCommentRequest>
{
    public AddSpotCommentRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
        RuleFor(x => x.Dto).NotEmpty();
        RuleFor(x => x.Dto.Text).NotEmpty();
    }
}