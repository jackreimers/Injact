namespace Injact.Options;

public interface IOptions<out T>
{
    public T Value { get; }
}