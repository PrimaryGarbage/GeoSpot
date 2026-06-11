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

    [HttpPut("{id:guid}/view")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> AddSpotView([FromRoute] Guid id, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<AddSpotViewRequest, Empty>(new AddSpotViewRequest(id), ct);

        return NoContent();
    }
    
    [HttpPut("{id:guid}/reaction")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> AddSpotReaction([FromRoute] Guid id, [FromBody] AddSpotReactionRequestDto dto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<AddSpotReactionRequest, Empty>(new AddSpotReactionRequest(id, dto), ct);
        
        return NoContent();
    }
    
    [HttpDelete("{id:guid}/reaction")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> RemoveSpotReaction([FromRoute] Guid id, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<RemoveSpotReactionRequest, Empty>(new RemoveSpotReactionRequest(id), ct);

        return NoContent();
    }

    [HttpPost("{id:guid}/comments")]
    [ProducesOkResponse<AddSpotCommentResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> AddSpotComment([FromRoute] Guid id, [FromBody] AddSpotCommentRequestDto dto, CancellationToken ct)
    {
        var result = await _dispatcher.DispatchAsync<AddSpotCommentRequest, AddSpotCommentResponseDto>(new AddSpotCommentRequest(id, dto), ct);

        return Ok(result);
    }

    [HttpDelete("comments/{id:guid}")]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> RemoveSpotComment([FromRoute] Guid id, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<RemoveSpotCommentRequest, Empty>(new RemoveSpotCommentRequest(id), ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/comments")]
    [ProducesOkResponse<GetSpotCommentsResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetSpotComments([FromRoute] Guid id, CancellationToken ct)
    {
        var result = await _dispatcher.DispatchAsync<GetSpotCommentsRequest, GetSpotCommentsResponseDto>(new GetSpotCommentsRequest(id), ct);

        return Ok(result);
    }

    [HttpGet("{id:guid}/categories")]
    [ProducesOkResponse<GetSpotCategoriesResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetSpotCategories(Guid id, CancellationToken ct)
    {
        var result = await _dispatcher.DispatchAsync<GetSpotCategoriesRequest, GetSpotCategoriesResponseDto>(new GetSpotCategoriesRequest(id), ct);

        return Ok(result);
    }

    [HttpPut("{id:guid}/categories")]
    [ProducesOkResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UpdateSpotCategories(Guid id, UpdateSpotCategoriesRequestDto dto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UpdateSpotCategoriesRequest, Empty>(new UpdateSpotCategoriesRequest(id, dto), ct);

        return Ok();
    }
}