using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class RemoveSpotCommentRequestValidator : AbstractValidator<RemoveSpotCommentRequest>
{
    public RemoveSpotCommentRequestValidator()
    {
        RuleFor(x => x.CommentId).NotEmpty();
    }
}