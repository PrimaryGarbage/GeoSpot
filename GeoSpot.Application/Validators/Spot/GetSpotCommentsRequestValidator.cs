using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class GetSpotCommentsRequestValidator : AbstractValidator<GetSpotCommentsRequest>
{
    public GetSpotCommentsRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}