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
    
    public async Task<IActionResult> SendVerificationCodeAsync(SendVerificationCodeDto dto, CancellationToken ct)
    {
        await _mediator.Send(new SendVerificationCodeRequest(dto), ct);
        
        return Created();
    }
}