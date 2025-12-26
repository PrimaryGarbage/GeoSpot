using FluentValidation;
using GeoSpot.Application.Handlers.Auth;

namespace GeoSpot.Application.Validators;

[ExcludeFromCodeCoverage]
public class SendVerificationCodeRequestValidator : AbstractValidator<SendVerificationCodeRequest>
{
    public SendVerificationCodeRequestValidator()
    {
        RuleFor(x => x.Dto.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}