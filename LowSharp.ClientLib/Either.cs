using System.Diagnostics.CodeAnalysis;

namespace LowSharp.ClientLib;

public sealed class Either<TSuccess, TFailure>
{
    private readonly TSuccess? _success;
    private readonly TFailure? _failure;

    public bool IsSuccess { get; }

    private Either(TSuccess success)
    {
        _success = success;
        IsSuccess = true;
    }
    private Either(TFailure failure)
    {
        _failure = failure;
        IsSuccess = false;
    }

    public static implicit operator Either<TSuccess, TFailure>(TSuccess success)
        => new Either<TSuccess, TFailure>(success);

    public static implicit operator Either<TSuccess, TFailure>(TFailure failure)
        => new Either<TSuccess, TFailure>(failure);

    public bool TryGetSuccess([NotNullWhen(true)] out TSuccess? success)
    {
        success = _success!;
        return IsSuccess;
    }

    public bool TryGetFailure([NotNullWhen(true)] out TFailure? failure)
    {
        failure = _failure!;
        return !IsSuccess;
    }

    public void Map(Action<TSuccess> success, Action<TFailure> failure)
    {
        if (IsSuccess)
            success(_success!);
        else
            failure(_failure!);
    }

    public async Task MapAsync(Func<TSuccess, Task> success, Func<TFailure, Task> failure)
    {
        if (IsSuccess)
            await success(_success!);
        else
            await failure(_failure!);
    }
}
