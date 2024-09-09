namespace Injact.Container.Binding;

public class FactoryBinding
{
    public Type InterfaceType { get; }
    public Type ConcreteType { get; }
    public Type ObjectType { get; }

    public List<Type> AllowedInjectionTypes { get; } = new();

    public FactoryBinding(Type interfaceType, Type concreteType, Type objectType)
    {
        InterfaceType = interfaceType;
        ConcreteType = concreteType;
        ObjectType = objectType;
    }
}