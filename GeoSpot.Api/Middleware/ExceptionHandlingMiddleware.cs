using FluentValidation;
using GeoSpot.Common.Exceptions;
using GeoSpot.Contracts.Error.v1;

namespace GeoSpot.Api.Middleware;

[ExcludeFromCodeCoverage]
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (InternalProblemException ex)
        {
            _logger.LogError(ex, "Application encountered InternalProblem error.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new InternalProblemResponseDto { Details = "Application encountered internal error." });
        }
        catch (BadRequestException ex)
        {
            _logger.LogError(ex, "Application encountered BadRequest error.");

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new BadRequestResponseDto { Details = ex.Message });
        }
        catch(NotFoundException ex)
        {
            _logger.LogError(ex, "Application encountered NotFound error.");
                
            context.Response.StatusCode = StatusCodes.Status404NotFound;
            await context.Response.WriteAsJsonAsync(new NotFoundResponseDto { Details = ex.Message });
        }
        catch(ValidationException ex)
        {
            _logger.LogError(ex, "Application encountered validation error.");
                
            IEnumerable<ValidationError> errors = ex.Errors.Select(e =>
                new ValidationError(field: e.PropertyName, error: e.ErrorMessage));
            
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new ValidationErrorResponseDto { Errors = errors });
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Application encountered unexpected error.");
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new InternalProblemResponseDto { Details = "Application encountered unexpected error." });
        }
    }
}