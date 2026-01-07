using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Auth;
using GeoSpot.Contracts.Auth;
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
    public async Task<IActionResult> SendVerificationCodeAsync(SendVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        await _dispatcher.Dispatch<SendVerificationCodeRequest, Empty>(new SendVerificationCodeRequest(requestDto), ct);
        return Created();
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyVerificationCodeAsync(VerifyVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        VerifyVerificationCodeResponseDto response = 
            await _dispatcher.Dispatch<VerifyVerificationCodeRequest, VerifyVerificationCodeResponseDto>(new VerifyVerificationCodeRequest(requestDto), ct);

        return Ok(response);
    }
}