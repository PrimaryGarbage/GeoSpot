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

    [HttpGet("{id:guid}")]
    [ProducesOkResponse<SpotDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetSpotById([FromRoute] Guid id, CancellationToken ct)
    {
        SpotDto result =
            await _dispatcher.DispatchAsync<GetSpotByIdRequest, SpotDto>(new GetSpotByIdRequest(id), ct);

        return Ok(result);
    }

    [HttpPost]
    [ProducesOkResponse<SpotDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> CreateSpot([FromBody] CreateSpotRequestDto dto, CancellationToken ct)
    {
        SpotDto result =
            await _dispatcher.DispatchAsync<CreateSpotRequest, SpotDto>(new CreateSpotRequest(dto), ct);

        return Ok(result);
    }

    [HttpPut("{id:guid}")]
    [ProducesOkResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UpdateSpot([FromRoute] Guid id, [FromBody] UpdateSpotRequestDto dto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UpdateSpotRequest, Empty>(new UpdateSpotRequest(id, dto), ct);

        return Ok();
    }

    [HttpDelete("{id:guid}")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> DeleteSpot([FromRoute] Guid id, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<DeleteSpotRequest, Empty>(new DeleteSpotRequest(id), ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/view")]
    [ProducesCreatedResponse]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> AddSpotView([FromRoute] Guid id, CancellationToken ct)
    {
        bool created = await _dispatcher.DispatchAsync<AddSpotViewRequest, bool>(new AddSpotViewRequest(id), ct);

        return created? Created() : NoContent();
    }
}