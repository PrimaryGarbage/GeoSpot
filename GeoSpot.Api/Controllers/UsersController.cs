using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.User;
using GeoSpot.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GeoSpot.Api.ApiVersionConstants;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/users")]
[ApiVersion(Version1_0)]
public class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public UsersController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpGet("me")]
    [Authorize]
    [ProducesOkResponse<UserDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        UserDto user = await _dispatcher.DispatchAsync<GetCurrentUserRequest, UserDto>(new GetCurrentUserRequest(), ct);
        
        return Ok(user);
    }

    [HttpPut("me")]
    [Authorize]
    [ProducesOkResponse<UserDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateCurrentUserRequestDto dto, CancellationToken ct)
    {
        UserDto user = await _dispatcher.DispatchAsync<UpdateCurrentUserRequest, UserDto>(new UpdateCurrentUserRequest(dto), ct);

        return Ok(user);
    }

    [HttpPut("me/location")]
    [Authorize]
    [ProducesOkResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UpdateCurrentUserLocation([FromBody] UpdateCurrentUserLocationRequestDto dto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UpdateCurrentUserLocationRequest, Empty>(new UpdateCurrentUserLocationRequest(dto), ct);

        return Ok();
    }

    [HttpGet("me/categories")]
    [Authorize]
    [ProducesOkResponse<GetCurrentUserCategoriesResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetCurrentUserCategories(CancellationToken ct)
    {
        GetCurrentUserCategoriesResponseDto result = await _dispatcher.DispatchAsync<GetCurrentUserCategoriesRequest, GetCurrentUserCategoriesResponseDto>(
            new GetCurrentUserCategoriesRequest(), ct);

        return Ok(result);
    }

    [HttpPut("me/categories")]
    [Authorize]
    [ProducesOkResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> UpdateCurrentUserCategories(UpdateCurrentUserCategoriesRequestDto dto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<UpdateCurrentUserCategoriesRequest, Empty>(new UpdateCurrentUserCategoriesRequest(dto), ct);

        return Ok();
    }
}