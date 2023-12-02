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

    public FactoryBindingBuilder WithType<TFactory, TObject>() where TFactory : IFactory<TObject>
    {
        _statement.InterfaceType = typeof(IFactory<TObject>);
        _statement.ConcreteType = typeof(TFactory);
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

    public FactoryBindingStatement Finalise()
    {
        _callback(_statement);
        return _statement;
    }
}