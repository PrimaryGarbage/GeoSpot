using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class GetSpotByIdRequestValidator : AbstractValidator<GetSpotByIdRequest>
{
    public GetSpotByIdRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}