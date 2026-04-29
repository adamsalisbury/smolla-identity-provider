namespace Smolla.IdentityProvider.Application.Common;

/// <summary>
/// Lightweight result type for operations that may fail with a set of human
/// readable error messages, without resorting to exceptions for control flow.
/// </summary>
/// <typeparam name="TValue">The value type returned on success.</typeparam>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Design",
    "CA1000:Do not declare static members on generic types",
    Justification = "Static factory methods are the idiomatic constructor pattern for result types.")]
public readonly record struct OperationResult<TValue>
{
    private OperationResult(bool succeeded, TValue? value, IReadOnlyCollection<string> errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public TValue? Value { get; }

    public IReadOnlyCollection<string> Errors { get; }

    public static OperationResult<TValue> Success(TValue value)
    {
        return new OperationResult<TValue>(true, value, []);
    }

    public static OperationResult<TValue> Failure(params string[] errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new OperationResult<TValue>(false, default, errors);
    }

    public static OperationResult<TValue> Failure(IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);
        return new OperationResult<TValue>(false, default, [.. errors]);
    }
}
