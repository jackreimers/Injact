namespace Injact;

public sealed class Factory<TInterface> : IFactory<TInterface>
{
    private readonly DiContainer _container;

    public Factory(DiContainer container)
    {
        _container = container;
    }

    public TInterface Create(bool deferInitialisation = false)
    {
        return (TInterface)_container.Create(typeof(TInterface), deferInitialisation, Array.Empty<object>());
    }

    public TInterface Create(params object[] args)
    {
        return (TInterface)_container.Create(typeof(TInterface), false, args);
    }

    public TInterface Create(bool deferInitialisation, params object[] args)
    {
        return (TInterface)_container.Create(typeof(TInterface), deferInitialisation, args);
    }

    public TInterface Create<TConcrete>(bool deferInitialisation = false)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), deferInitialisation, Array.Empty<object>());
    }

    public TInterface Create<TConcrete>(params object[] args)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), false, args);
    }

    public TInterface Create<TConcrete>(bool deferInitialisation, params object[] args)
        where TConcrete : class, TInterface
    {
        return (TInterface)_container.Create(typeof(TConcrete), deferInitialisation, args);
    }
}
