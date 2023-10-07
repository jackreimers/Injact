namespace Injact.Injection;

public interface IFactory { }

public interface IFactory<out TValue> : IFactory
{
    public TValue Create();
}