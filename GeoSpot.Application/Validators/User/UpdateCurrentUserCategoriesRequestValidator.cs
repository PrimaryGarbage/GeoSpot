using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.User;

namespace GeoSpot.Application.Validators.User;

public class UpdateCurrentUserCategoriesRequestValidator : AbstractValidator<UpdateCurrentUserCategoriesRequest>
{
    public UpdateCurrentUserCategoriesRequestValidator()
    {
        When(x => x.Dto.Categories.Any(), () =>
        {
            RuleForEach(x => x.Dto.Categories)
                .ChildRules(x => x
                    .RuleFor(y => y.CategoryId)
                        .NotEmpty());
        });
    }
}