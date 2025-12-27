using GeoSpot.Application.Handlers.Auth;
using GeoSpot.Contracts.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[ExcludeFromCodeCoverage]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpPost("send-code")]
    public async Task<IActionResult> SendVerificationCodeAsync(SendVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        await _mediator.Send(new SendVerificationCodeRequest(requestDto), ct);
        
        return Created();
    }

    [HttpPost("verify-code")]
    public async Task<IActionResult> VerifyVerificationCodeAsync(VerifyVerificationCodeRequestDto requestDto, CancellationToken ct)
    {
        VerifyVerificationCodeResponseDto response = await _mediator.Send(new VerifyVerificationCodeRequest(requestDto), ct);

        return Ok(response);
    }
}