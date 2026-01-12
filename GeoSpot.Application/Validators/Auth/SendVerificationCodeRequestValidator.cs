using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Auth;

namespace GeoSpot.Application.Validators.Auth;

public class SendVerificationCodeRequestValidator : AbstractValidator<SendVerificationCodeRequest>
{
    public SendVerificationCodeRequestValidator()
    {
        RuleFor(x => x.RequestDto.PhoneNumber)
            .NotEmpty()
            .MaximumLength(20);
    }
}