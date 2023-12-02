namespace Injact;

public class ObjectBindingBuilder
{
    private readonly Action<IBindingStatement> _callback;
    private readonly ObjectBindingStatement _statement = new();

    public ObjectBindingBuilder(Action<IBindingStatement> callback)
    {
        _callback = callback;
    }
    
    public ObjectBindingBuilder WithType<TInterface, TConcrete>() where TConcrete : class, TInterface
    {
        _statement.InterfaceType = typeof(TInterface);
        _statement.ConcreteType = typeof(TConcrete);
        return this;
    }

    public ObjectBindingBuilder FromInstance(object instance)
    {
        Guard.Against.Null(instance, $"Null instance reference on {_statement.InterfaceType.Name} binding!");

        _statement.Flags |= StatementFlags.Singleton;
        _statement.Instance = instance;
        return this;
    }

    public ObjectBindingBuilder AsSingleton()
    {
        _statement.Flags |= StatementFlags.Singleton;
        return this;
    }

    public ObjectBindingBuilder AsTransient()
    {
        _statement.Flags &= ~StatementFlags.Singleton;
        return this;
    }

    public ObjectBindingBuilder Immediate()
    {
        _statement.Flags |= StatementFlags.Immediate;
        return this;
    }

    public ObjectBindingBuilder Delayed()
    {
        _statement.Flags &= ~StatementFlags.Immediate;
        return this;
    }
    
    public ObjectBindingBuilder WhenInjectedInto<TValue>()
    {
        _statement.AllowedInjectionTypes.Add(typeof(TValue));
        return this;
    }

    public ObjectBindingBuilder WhenInjectedInto(Type allowedType)
    {
        _statement.AllowedInjectionTypes.Add(allowedType);
        return this;
    }

    public ObjectBindingStatement Finalise()
    {
        _callback.Invoke(_statement);
        return _statement;
    }
}