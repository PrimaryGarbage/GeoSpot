using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Spot;
using GeoSpot.Contracts.Spot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GeoSpot.Api.ApiVersionConstants;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/spots")]
[ApiVersion(Version1_0)]
[Authorize]
public class SpotsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public SpotsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("nearby")]
    [ProducesOkResponse<SearchNearbySpotsResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> SearchNearbySpots([FromBody] SearchNearbySpotsRequestDto dto, CancellationToken ct)
    {
        SearchNearbySpotsResponseDto result =
            await _dispatcher.DispatchAsync<SearchNearbySpotsRequest, SearchNearbySpotsResponseDto>(new SearchNearbySpotsRequest(dto), ct);

        return Ok(result);
    }
}