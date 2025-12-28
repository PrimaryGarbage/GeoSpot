using GeoSpot.Application.Services.Interfaces;

namespace GeoSpot.Application.Services;

internal class SmsService : ISmsService
{
    public async Task SendSmsAsync(string phoneNumber, string message, CancellationToken ct)
    {
        // throw new NotImplementedException();
    }
}