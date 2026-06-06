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
    [ProducesOkResponse<Guid>]
    [ProducesNotFoundResponse]
    [ProducesBadRequestResponse]
    public async Task<IActionResult> RegisterDevice([FromBody] RegisterDeviceRequestDto dto, CancellationToken ct)
    {
        Guid tokenId = await _dispatcher.DispatchAsync<RegisterDeviceRequest, Guid>(new RegisterDeviceRequest(dto), ct);
        
        return Ok(tokenId);
    }
    
    [HttpDelete("{deviceTokenId:guid}")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    public async Task<IActionResult> UnregisterDevice([FromRoute] Guid deviceTokenId, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UnregisterDeviceRequest, Empty>(new UnregisterDeviceRequest(deviceTokenId), ct);
        
        return NoContent();
    }
}