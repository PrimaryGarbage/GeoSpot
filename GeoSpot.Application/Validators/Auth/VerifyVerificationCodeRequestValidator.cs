using FluentValidation;
using GeoSpot.Application.Handlers.Auth;

namespace GeoSpot.Application.Validators.Auth;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeRequestValidator : AbstractValidator<VerifyVerificationCodeRequest>
{
    public VerifyVerificationCodeRequestValidator()
    {
        RuleFor(x => x.RequestDto.VerificationCodeId).NotEmpty();
        RuleFor(x => x.RequestDto.VerificationCode)
            .NotEmpty()
            .Length(6);
    }
}