namespace Injact;

public class FactoryBindingBuilder
{
    private readonly Action<IBindingStatement> _callback;
    private readonly FactoryBindingStatement _statement = new();

    public FactoryBindingBuilder(Action<IBindingStatement> callback)
    {
        _callback = callback;
        _statement.Flags |= StatementFlags.Factory;
    }

    public FactoryBindingBuilder WithType<TInterface, TFactory, TObject>() 
        where TInterface : IFactory
        where TFactory : TInterface
    {
        _statement.InterfaceType = typeof(TInterface);
        _statement.ConcreteType = typeof(TFactory);
        _statement.ObjectType = typeof(TObject);
     
        return this;
    }

    public FactoryBindingBuilder WhenInjectedInto<TValue>()
    {
        _statement.AllowedInjectionTypes.Add(typeof(TValue));
        return this;
    }

    public FactoryBindingBuilder WhenInjectedInto(Type allowedType)
    {
        _statement.AllowedInjectionTypes.Add(allowedType);
        return this;
    }

    public void Finalise()
    {
        _callback(_statement);
    }
}