namespace HotelListing.API.Results;

public enum ErrorType
{
    Failure = 0,
    Validation = 1,
    NotFound = 2,
    Conflict = 3,
    Unauthorized = 4
}

public readonly record struct Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.Failure);
    public bool IsNone => string.IsNullOrWhiteSpace(Code);

    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);
    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);
    public static implicit operator Result(Error error) => Result.Failure(error);
}

public readonly record struct Result
{
    private readonly Error[]? _errors;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors => _errors ?? Array.Empty<Error>();

    private Result(bool isSuccess, Error[] errors)
    {
        IsSuccess = isSuccess;
        _errors = errors;
    }

    public static Result Success() => new(true, Array.Empty<Error>());
    public static Result Failure(params Error[] errors) => new(false, errors);

    public static Result Combine(params Result[] results)
    {
        var errors = results.SelectMany(r => r.Errors).ToArray();
        return errors.Length == 0 ? Success() : Failure(errors);
    }
}

public readonly record struct Result<T>
{
    private readonly T? _value;
    private readonly Error[]? _errors;

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error[] Errors => _errors ?? Array.Empty<Error>();

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("Impossible d'accéder à la valeur d'un résultat en échec.");

    private Result(bool isSuccess, T? value, Error[] errors)
    {
        IsSuccess = isSuccess;
        _value = value;
        _errors = errors;
    }

    public static Result<T> Success(T value) => new(true, value, Array.Empty<Error>());
    public static Result<T> Failure(params Error[] errors) => new(false, default, errors);

    public Result<K> Map<K>(Func<T, K> map)
        => IsSuccess ? Result<K>.Success(map(Value)) : Result<K>.Failure(Errors);

    public Result<K> Bind<K>(Func<T, Result<K>> next)
        => IsSuccess ? next(Value) : Result<K>.Failure(Errors);

    public Result<T> Ensure(Func<T, bool> predicate, Error error)
        => IsSuccess && !predicate(Value) ? Failure(error) : this;
}