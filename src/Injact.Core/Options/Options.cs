namespace Injact.Core;

public class Options<T> : IOptions<T>
{
    public T Value { get; }

    public Options(T value)
    {
        Value = value;
    }
}