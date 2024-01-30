namespace Injact;

public sealed class Factory<TValue> : BaseFactory, IFactory<TValue>
{
    public Factory(DiContainer container) 
        : base(container) { }

    public TValue Create()
    {
        return (TValue)_container.Create(typeof(TValue));
    }

    public TValue Create(params object[] args)
    {
        return (TValue)_container.Create(typeof(TValue), args);
    }
}