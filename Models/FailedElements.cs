using FluentResults;

namespace CustomPizzaApi.Models;

public class BadRequest<T> : Error, CustomError<T>
{
    public ICollection<T> Values { get; set; } = [];
    public BadRequest() : base(FailCause.BadRequest.ToString()) { }
}

public class Conflict<T> : Error, CustomError<T>
{
    public ICollection<T> Values { get; set; } = [];
    public Conflict() : base(FailCause.Conflict.ToString()) { }
}

public class NotFound<T> : Error, CustomError<T>
{
    public ICollection<T> Values { get; set; } = [];
    public NotFound() : base(FailCause.NotFound.ToString()) { }
}

public interface CustomError<T> : IError
{
    public ICollection<T> Values { get; set; }
    public int Count { get => Values.Count; }

    public void AddMultiple(List<T> values)
    {
        foreach (var value in values) Values.Add(value);
    }
    public void Add(T value) => Values.Add(value);
}

public enum FailCause
{
    NotFound,
    Conflict,
    BadRequest,
}

