namespace eMechanic.Common.Result;

using Microsoft.AspNetCore.Http;

public static class ErrorMapper
{
    public static IResult MapToHttpResult(Error error) => error.Code switch
    {
        EErrorCode.NotFoundError => Results.NotFound(error),
        EErrorCode.ValidationError => HandleValidationError(error),
        EErrorCode.UnauthorizedError => Results.Unauthorized(),

        _ => Results.Problem(
            title: error.Code.ToString(),
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError)
    };

    private static IResult HandleValidationError(Error error)
    {
        if (error.ValidationErrors != null && error.ValidationErrors.Any())
        {
            return Results.ValidationProblem(
                statusCode: StatusCodes.Status400BadRequest,
                errors: (IDictionary<string, string[]>)error.ValidationErrors
            );
        }

        var generalErrors = new Dictionary<string, string[]>
        {
            { "General", new[] { error.Message ?? "Validation error" } }
        };

        return Results.ValidationProblem(
            statusCode: StatusCodes.Status400BadRequest,
            errors: generalErrors
        );
    }
}
