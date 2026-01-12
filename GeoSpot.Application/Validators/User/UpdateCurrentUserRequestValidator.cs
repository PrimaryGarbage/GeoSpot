using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.User;

namespace GeoSpot.Application.Validators.User;

public sealed class UpdateCurrentUserRequestValidator : AbstractValidator<UpdateCurrentUserRequest>
{
    public UpdateCurrentUserRequestValidator()
    {
        RuleFor(x => x.Dto.Email)
            .EmailAddress()
            .When(x => x.Dto.Email is not null);
        RuleFor(x => x.Dto.BirthYear).NotEmpty();
        RuleFor(x => x.Dto.DisplayName).NotEmpty();
    }
}