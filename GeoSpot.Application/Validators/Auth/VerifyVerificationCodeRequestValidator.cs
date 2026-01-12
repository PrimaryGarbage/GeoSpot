using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Auth.v1;
using GeoSpot.Common.ConfigurationSections;
using Microsoft.Extensions.Options;

namespace GeoSpot.Application.Validators.Auth;

[ExcludeFromCodeCoverage]
public class VerifyVerificationCodeRequestValidator : AbstractValidator<VerifyVerificationCodeRequest>
{
    public VerifyVerificationCodeRequestValidator(IOptions<VerificationCodeConfigurationSection> configuration)
    {
        RuleFor(x => x.Dto.PhoneNumber).NotEmpty();
        RuleFor(x => x.Dto.VerificationCode)
            .NotEmpty()
            .Length(configuration.Value.NumberOfDigits);
    }
}