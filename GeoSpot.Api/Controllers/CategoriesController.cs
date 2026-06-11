using Asp.Versioning;
using GeoSpot.Application.Dispatcher;
using GeoSpot.Application.Dispatcher.Handlers.Category;
using GeoSpot.Contracts.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api.Controllers;

[ApiController]
[Route("api/categories")]
[ApiVersion(ApiVersionConstants.Version1_0)]
[Authorize]
public class CategoriesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public CategoriesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet]
    [ProducesOkResponse<GetCategoriesResponseDto>]
    [ProducesUnauthorizedResponse]
    public async Task<IActionResult> GetCategories(CancellationToken ct)
    {
        var result = await _dispatcher.DispatchAsync<GetCategoriesRequest, GetCategoriesResponseDto>(new GetCategoriesRequest(), ct);
        
        return Ok(result);
    }
}