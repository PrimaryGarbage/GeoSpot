using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Device;
using GeoSpot.Contracts.Device;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/devices")]
[ApiVersion(ApiVersionConstants.Version1_0)]
[Authorize]
public class DevicesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public DevicesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("register")]
    [ProducesOkResponse<RegisterDeviceResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesBadRequestResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequestDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.DispatchAsync<RegisterDeviceRequest, RegisterDeviceResponseDto>(new RegisterDeviceRequest(dto), ct);
        
        return Ok(result);
    }
    
    [HttpDelete("{deviceTokenId:guid}")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UnregisterDevice([FromRoute] Guid deviceTokenId, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UnregisterDeviceRequest, Empty>(new UnregisterDeviceRequest(deviceTokenId), ct);
        
        return NoContent();
    }
}