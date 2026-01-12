using System.Net.Mime;
using GeoSpot.Contracts.Error;
using Microsoft.AspNetCore.Mvc;

namespace GeoSpot.Api;

internal class EmptyContent;

[ExcludeFromCodeCoverage]
internal class ProducesOkResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesOkResponseAttribute() : base(StatusCodes.Status200OK)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesOkResponseAttribute<TResponseType> : ProducesResponseTypeAttribute<TResponseType>
{
    public ProducesOkResponseAttribute() : base(StatusCodes.Status200OK, MediaTypeNames.Application.Json)
    { }
}

[ExcludeFromCodeCoverage]
internal class ProducesCreatedResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesCreatedResponseAttribute() : base(StatusCodes.Status201Created)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesNoContentResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesNoContentResponseAttribute() : base(StatusCodes.Status204NoContent)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesBadRequestResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesBadRequestResponseAttribute() : base(typeof(BadRequestResponseDto), StatusCodes.Status400BadRequest, 
        MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesNotFoundResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesNotFoundResponseAttribute() : base(typeof(NotFoundResponseDto), StatusCodes.Status404NotFound,
    MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesInternalProblemResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesInternalProblemResponseAttribute() : base(typeof(InternalProblemResponseDto), StatusCodes.Status500InternalServerError,
        MediaTypeNames.Application.Json)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesValidationErrorResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesValidationErrorResponseAttribute() : base(typeof(ValidationErrorResponseDto), StatusCodes.Status400BadRequest, 
        MediaTypeNames.Application.ProblemJson)
    {}
}

[ExcludeFromCodeCoverage]
internal class ProducesUnauthorizedResponseAttribute : ProducesResponseTypeAttribute
{
    public ProducesUnauthorizedResponseAttribute() : base(typeof(EmptyContent), StatusCodes.Status401Unauthorized)
    { }
}
