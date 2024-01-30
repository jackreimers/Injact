namespace Injact;

public abstract class BaseFactory : IFactory
{
    protected readonly DiContainer _container;

    protected BaseFactory(DiContainer container)
    {
        _container = container;
    }
}