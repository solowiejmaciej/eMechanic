namespace eMechanic.Common.Result;

using Microsoft.AspNetCore.Http;
using System;

public static class ResultExtensions
{
    public static IResult ToStatusCode<TValue>(
        this Result<TValue, Error> result,
        Func<TValue, IResult> successAction,
        Func<Error, IResult> errorMapper)
    {
        if (result.HasError())
        {
            return errorMapper(result.Error!);
        }

        return successAction(result.Value!);
    }
}
