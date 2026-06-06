using FluentValidation;
using GeoSpot.Application.Dispatcher.Handlers.Device;
using GeoSpot.Common.Enums;

namespace GeoSpot.Application.Validators.Device;

public class RegisterDeviceRequestValidator : AbstractValidator<RegisterDeviceRequest>
{
    public RegisterDeviceRequestValidator()
    {
        RuleFor(x => x.Dto.Platform).IsInEnum();
        RuleFor(x => x.Dto.Token).NotEmpty();
    }
}