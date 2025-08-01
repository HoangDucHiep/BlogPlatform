using System.Diagnostics.CodeAnalysis;

namespace Lumo.Domain.Abstractions;

public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    protected Result(bool isSuccess, Error? error)
    {
        if (isSuccess && error != null)
        {
            throw new InvalidOperationException();
        }

        if (!isSuccess && error == null)
        {
            throw new InvalidOperationException();
        }

        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success()
    {
        return new Result(true, null);
    }

    public static Result Failure(Error error)
    {
        return new Result(false, error);
    }

    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, null);

    public static Result<TValue> Failure<TValue>(Error error) => new(default, false, error);

    public static Result<TValue> Create<TValue>(TValue? value) =>
        value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
}

public class Result<TValue> : Result
{
    private readonly TValue? _value;

    public Result(TValue? value, bool isSuccess, Error? error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    [NotNull]
    public TValue Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException("The value of a failure result can not be accessed.");

    public static implicit operator Result<TValue>(TValue? value) => Create(value);
}

//public class ResultJsonConverter<T> : JsonConverter<Result<T>>
//{
//    public override Result<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
//    {
//        throw new NotImplementedException("Deserialization not implemented");
//    }

//    public override void Write(Utf8JsonWriter writer, Result<T> value, JsonSerializerOptions options)
//    {
//        writer.WriteStartObject();
//        writer.WriteBoolean("success", value.IsSuccess);

//        if (value.IsFailure && value.Error != null)
//        {
//            writer.WritePropertyName("error");
//            JsonSerializer.Serialize(writer, value.Error, options);
//        }
//        else if (value.IsSuccess)
//        {
//            writer.WritePropertyName("data");
//            JsonSerializer.Serialize(writer, value.Value, options);
//        }

//        writer.WriteEndObject();
//    }
//}

//public class ResultJsonConverterFactory : JsonConverterFactory
//{
//    public override bool CanConvert(Type typeToConvert)
//    {
//        if (!typeToConvert.IsGenericType)
//        {
//            return false;
//        }

//        var genericType = typeToConvert.GetGenericTypeDefinition();
//        return genericType == typeof(Result<>);
//    }

//    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
//    {
//        var valueType = typeToConvert.GetGenericArguments()[0];
//        var converterType = typeof(ResultJsonConverter<>).MakeGenericType(valueType);

//        return (JsonConverter)Activator.CreateInstance(converterType)!;
//    }
//}
