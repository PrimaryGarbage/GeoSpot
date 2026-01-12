using System.Net.Mime;
using GeoSpot.Contracts.Error;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api;

[ExcludeFromCodeCoverage]
public class ProducesOkResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesOkResponseAttribute() : base(StatusCodes.Status200OK)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesOkResponseAttribute<TResponseType> : ProducesResponseTypeAttribute<TResponseType>
{
    public ProducesOkResponseAttribute() : base(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
    { }
}

[ExcludeFromCodeCoverage]
public class ProducesCreatedResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesCreatedResponseAttribute() : base(StatusCodes.Status201Created)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesNoContentResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesNoContentResponseAttribute() : base(StatusCodes.Status204NoContent)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesBadRequestResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesBadRequestResponseAttribute() : base(typeof(BadRequestResponseDto), StatusCodes.Status400BadRequest, 
        MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesNotFoundResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesNotFoundResponseAttribute() : base(typeof(NotFoundResponseDto), StatusCodes.Status404NotFound,
    MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesInternalProblemResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesInternalProblemResponseAttribute() : base(typeof(InternalProblemResponseDto), StatusCodes.Status500InternalServerError,
        MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
public class ProducesValidationErrorResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesValidationErrorResponseAttribute() : base(typeof(ValidationErrorResponseDto), StatusCodes.Status400BadRequest, 
        MediaTypeNames.Application.ProblemJson)
    {}
}