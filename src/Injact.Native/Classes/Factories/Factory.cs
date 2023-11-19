namespace Injact;

public class Factory<TValue> : IFactory<TValue>
{
    private readonly DiContainer _container;
    
    public Factory(DiContainer container)
    {
        _container = container;
    }

    public virtual TValue Create()
    {
        return (TValue)_container.Create(typeof(TValue));
    }
}