using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class GetSpotCategoriesRequestValidator : AbstractValidator<GetSpotCategoriesRequest>
{
    public GetSpotCategoriesRequestValidator()
    {
        RuleFor(x => x.SpotId).NotEmpty();
    }
}