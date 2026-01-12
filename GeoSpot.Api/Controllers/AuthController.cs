using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Auth;
using GeoSpot.Contracts.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static GeoSpot.Api.ApiVersionConstants;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/auth")]
[ExcludeFromCodeCoverage]
[ApiVersion(Version1_0)]
public class AuthController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public AuthController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }
    
    [HttpPost("send-code")]
    [ProducesCreatedResponse]
    [ProducesBadRequestResponse]
    [ProducesValidationErrorResponse]
    public async Task<IActionResult> SendVerificationCodeAsync([FromBody] SendVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<SendVerificationCodeRequest, Empty>(new SendVerificationCodeRequest(requestDto), ct);
        return Created();
    }

    [HttpPost("verify-code")]
    [ProducesOkResponse<VerifyVerificationCodeResponseDto>]
    [ProducesNotFoundResponse]
    [ProducesBadRequestResponse]
    [ProducesValidationErrorResponse]
    public async Task<IActionResult> VerifyVerificationCodeAsync([FromBody] VerifyVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        VerifyVerificationCodeResponseDto response = 
            await _dispatcher.DispatchAsync<VerifyVerificationCodeRequest, VerifyVerificationCodeResponseDto>(new VerifyVerificationCodeRequest(requestDto), ct);

        return Ok(response);
    }

    [HttpPost("refresh")]
    [ProducesResponseType<AccessTokenDto>(StatusCodes.Status200OK)]
    [ProducesNotFoundResponse]
    [ProducesBadRequestResponse]
    [ProducesValidationErrorResponse]
    public async Task<IActionResult> RefreshAccessToken([FromBody] RefreshAccessTokenRequestDto requestDto, CancellationToken ct)
    {
        AccessTokenDto response = 
            await _dispatcher.DispatchAsync<RefreshAccessTokenRequest, AccessTokenDto>(new RefreshAccessTokenRequest(requestDto.RefreshToken), ct);

        return Ok(response);
    }

    [HttpPost("logout")]
    [Authorize]
    [ProducesNoContentResponse]
    [ProducesNotFoundResponse]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> LogoutUser(CancellationToken ct)
    {
        await _dispatcher.DispatchAsync<LogoutUserRequest, Empty>(new LogoutUserRequest(), ct);

        return NoContent();
    }
}