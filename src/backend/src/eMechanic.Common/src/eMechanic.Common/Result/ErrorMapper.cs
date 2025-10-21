namespace eMechanic.Common.Result;

using Microsoft.AspNetCore.Http;

public static class ErrorMapper
{
    public static IResult MapToHttpResult(Error error) => error.Code switch
    {
        EErrorCode.NotFoundError => Results.NotFound(error),
        EErrorCode.ValidationError => Results.ValidationProblem(
            statusCode: StatusCodes.Status400BadRequest,
            errors: (IDictionary<string, string[]>)error.ValidationErrors!
        ),
        _ => Results.Problem(
            title: error.Code.ToString(),
            detail: error.Message,
            statusCode: StatusCodes.Status500InternalServerError)
    };
}
