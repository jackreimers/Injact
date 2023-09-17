namespace Injact;

public class Factory<TValue> : IFactory<TValue>
{
    [Inject] private readonly DiContainer _container;

    public TValue Create()
    {
        return _container.Resolve<TValue>(null);
    }
}