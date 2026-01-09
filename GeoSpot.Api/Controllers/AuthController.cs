using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Auth;
using GeoSpot.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ExcludeFromCodeCoverage]
public class AuthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("send-code")]
    public async Task<IActionResult> SendVerificationCodeAsync([FromBody] SendVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        await _dispatcher.Dispatch<SendVerificationCodeRequest, Empty>(new SendVerificationCodeRequest(requestDto), ct);
        return Created();
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyVerificationCodeAsync([FromBody] VerifyVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        AccessTokenDto response = 
            await _dispatcher.Dispatch<VerifyVerificationCodeRequest, AccessTokenDto>(new VerifyVerificationCodeRequest(requestDto), ct);

        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshAccessTokenRequestDto requestDto, CancellationToken ct)
    {
        AccessTokenDto response = 
            await _dispatcher.Dispatch<RefreshAccessTokenRequest, AccessTokenDto>(new RefreshAccessTokenRequest(requestDto.RefreshToken), ct);

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> LogoutUser(CancellationToken ct)
    {
        await _dispatcher.Dispatch<LogoutUserRequest, Empty>(new LogoutUserRequest(), ct);

        return Ok();
    }
}