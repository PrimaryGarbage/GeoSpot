using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.ReactionType;
using GeoSpot.Contracts.ReactionType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/reaction-types")]
[ApiVersion(ApiVersionConstants.Version1_0)]
[Authorize]
public class ReactionTypesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ReactionTypesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpGet]
    [ProducesOkResponse<GetReactionTypesResponseDto>]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetReactionTypes()
    {
        var response = await _dispatcher.DispatchAsync<GetReactionTypesRequest, GetReactionTypesResponseDto>(new());
        
        return Ok(response);
    }
}