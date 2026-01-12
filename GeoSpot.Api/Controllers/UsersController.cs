using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.User;
using GeoSpot.Contracts.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/users")]
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
}