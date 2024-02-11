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

    public ObjectBindingBuilder WhenInjectedInto(params Type[] allowed)
    {
        _statement.AllowedInjectionTypes.AddRange(allowed);
        return this;
    }

    public void Finalise()
    {
        _callback.Invoke(_statement);
    }
}