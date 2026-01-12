using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Auth.v1;

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