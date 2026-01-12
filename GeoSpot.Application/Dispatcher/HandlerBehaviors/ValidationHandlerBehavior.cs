using FluentValidation;
using FluentValidation.Results;

namespace GeoSpot.Application.Dispatcher.HandlerBehaviors;

internal class ValidationHandlerBehavior<TRequest, TResponse> : IHandlerBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    
    public ValidationHandlerBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (!_validators.Any()) return await next(ct);
        
        ValidationContext<TRequest> context = new(request);
        List<ValidationFailure> failures = _validators
            .Select(v => v.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(f => f != null)
            .ToList();
            
        if (failures.Count != 0)
            throw new ValidationException(failures);

        return await next(ct);
    }
}