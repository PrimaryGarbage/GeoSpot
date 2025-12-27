using FluentValidation;
using GeoSpot.Application.Handlers.Auth;

namespace GeoSpot.Application.Validators.Auth;

[ExcludeFromCodeCoverage]
public class SendVerificationCodeRequestValidator : AbstractValidator<SendVerificationCodeRequest>
{
    public SendVerificationCodeRequestValidator()
    {
        RuleFor(x => x.RequestDto.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}