namespace Injact;

public interface IFactory<TInterface> : IFactory
{
    public TInterface Create(bool deferInitialisation = false);

    public TInterface Create(params object[] args);

    public TInterface Create(bool deferInitialisation, params object[] args);

    public TInterface Create<TConcrete>(bool deferInitialisation = false)
        where TConcrete : class, TInterface;

    public TInterface Create<TConcrete>(params object[] args)
        where TConcrete : class, TInterface;

    public TInterface Create<TConcrete>(bool deferInitialisation, params object[] args)
        where TConcrete : class, TInterface;
}

public interface IFactory { }
