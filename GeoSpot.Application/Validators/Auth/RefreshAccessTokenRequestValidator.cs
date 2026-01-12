using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Auth.v1;

namespace GeoSpot.Application.Validators.Auth;

[ExcludeFromCodeCoverage]
public class RefreshAccessTokenRequestValidator : AbstractValidator<RefreshAccessTokenRequest>
{
    public RefreshAccessTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}