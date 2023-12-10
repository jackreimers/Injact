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

    public virtual TValue Create(params object[] args)
    {
        if (args.Length != args.Select(s => s.GetType()).Distinct().Count())
            throw new DependencyException("Cannot pass duplicate argument types to factory create method!");
        
        return (TValue)_container.Create(typeof(TValue), args);
    }
}