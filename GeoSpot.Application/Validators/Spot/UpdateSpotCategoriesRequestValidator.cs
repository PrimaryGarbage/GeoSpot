using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Spot;

namespace GeoSpot.Application.Validators.Spot;

public class UpdateSpotCategoriesRequestValidator : AbstractValidator<UpdateSpotCategoriesRequest>
{
    public UpdateSpotCategoriesRequestValidator()
    {
        RuleFor(x => x.SpodId).NotEmpty();
        RuleFor(x => x.Dto).NotEmpty();
        RuleFor(x => x.Dto.CategoryIds).NotNull();
    }
}