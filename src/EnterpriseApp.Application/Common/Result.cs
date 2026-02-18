namespace EnterpriseApp.Application.Common;

/// <summary>
/// Represents the result of an operation that can either succeed or fail.
/// </summary>
public class Result
{
    protected Result(bool isSuccess, string? error)
    {
        if (isSuccess && error != null)
        {
            throw new InvalidOperationException("Success result cannot have an error message.");
        }

        if (!isSuccess && string.IsNullOrWhiteSpace(error))
        {
            throw new InvalidOperationException("Failure result must have an error message.");
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool IsSuccess { get; }

    /// <summary>
    /// Indicates whether the operation failed.
    /// </summary>
    public bool IsFailure => !IsSuccess;

    /// <summary>
    /// The error message if the operation failed.
    /// </summary>
    public string? Error { get; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static Result Success() => new(true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    public static Result Failure(string error) => new(false, error);

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="value">The value.</param>
    public static Result<T> Success<T>(T value) => new(value, true, null);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <typeparam name="T">The value type.</typeparam>
    /// <param name="error">The error message.</param>
    public static Result<T> Failure<T>(string error) => new(default, false, error);
}

/// <summary>
/// Represents the result of an operation that returns a value and can either succeed or fail.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public class Result<T> : Result
{
    private readonly T? _value;

    protected internal Result(T? value, bool isSuccess, string? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value if the operation was successful.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if accessing value on a failed result.</exception>
    public T Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException("Cannot access value of a failed result.");
            }

            return _value!;
        }
    }

    /// <summary>
    /// Gets the value or a default if the operation failed.
    /// </summary>
    /// <param name="defaultValue">The default value.</param>
    public T GetValueOrDefault(T defaultValue = default!)
    {
        return IsSuccess ? _value! : defaultValue;
    }

    /// <summary>
    /// Implicit conversion from value to successful result.
    /// </summary>
    public static implicit operator Result<T>(T value) => Success(value);

    /// <summary>
    /// Maps the result value to a new type.
    /// </summary>
    /// <typeparam name="TNew">The new value type.</typeparam>
    /// <param name="mapper">The mapping function.</param>
    public Result<TNew> Map<TNew>(Func<T, TNew> mapper)
    {
        return IsSuccess
            ? Result.Success(mapper(Value))
            : Result.Failure<TNew>(Error!);
    }

    /// <summary>
    /// Binds the result to another result-returning operation.
    /// </summary>
    /// <typeparam name="TNew">The new value type.</typeparam>
    /// <param name="binder">The binding function.</param>
    public Result<TNew> Bind<TNew>(Func<T, Result<TNew>> binder)
    {
        return IsSuccess
            ? binder(Value)
            : Result.Failure<TNew>(Error!);
    }

    /// <summary>
    /// Executes an action on success.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public Result<T> OnSuccess(Action<T> action)
    {
        if (IsSuccess)
        {
            action(Value);
        }

        return this;
    }

    /// <summary>
    /// Executes an action on failure.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    public Result<T> OnFailure(Action<string> action)
    {
        if (IsFailure)
        {
            action(Error!);
        }

        return this;
    }

    /// <summary>
    /// Matches the result to one of two functions.
    /// </summary>
    /// <typeparam name="TResult">The result type.</typeparam>
    /// <param name="onSuccess">Function to execute on success.</param>
    /// <param name="onFailure">Function to execute on failure.</param>
    public TResult Match<TResult>(Func<T, TResult> onSuccess, Func<string, TResult> onFailure)
    {
        return IsSuccess ? onSuccess(Value) : onFailure(Error!);
    }
}
