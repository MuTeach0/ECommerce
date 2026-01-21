namespace ECommerce.Domain.Common.Results.Abstractions;

public interface IResult
{
    List<Error>? Errors { get; }
    public bool IsSuccess { get; }
}

public interface IResult<out TValue> : IResult
{
    TValue  Value { get; }
}