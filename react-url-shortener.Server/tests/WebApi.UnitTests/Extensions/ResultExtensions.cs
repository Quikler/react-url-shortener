using Shouldly;
using WebApi.Common;

namespace WebApi.UnitTests.Extensions;

public static class ResultExtensions
{
    public static TValue Success<TValue, TError>(this Result<TValue, TError> result)
    {
        result.IsSuccess.ShouldBeTrue();
        return result.Match(
            success => success,
            failure => throw new Exception("Should not be failure")
        );
    }

    public static TError Failure<TValue, TError>(this Result<TValue, TError> result)
    {
        result.IsError.ShouldBeTrue();
        return result.Match(
            success => throw new Exception("Should not be success"),
            failure => failure
        );
    }
}
