namespace Injact;

public interface IOptions<out T>
{
    public T Value { get; }
}