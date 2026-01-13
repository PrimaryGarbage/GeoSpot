using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.User;

namespace GeoSpot.Application.Validators.User;

public class UpdateCurrentUserLocationRequestValidator : AbstractValidator<UpdateCurrentUserLocationRequest>
{
    public UpdateCurrentUserLocationRequestValidator()
    {
        RuleFor(x => Math.Abs(x.Dto.Latitude)).LessThanOrEqualTo(90.0);
        RuleFor(x => Math.Abs(x.Dto.Longitude)).LessThanOrEqualTo(180.0);
    }
}