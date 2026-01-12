using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Auth;

namespace GeoSpot.Application.Validators.Auth;

public class RefreshAccessTokenRequestValidator : AbstractValidator<RefreshAccessTokenRequest>
{
    public RefreshAccessTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty();
    }
}