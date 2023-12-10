namespace Injact;

public class Binding
{
    public Type ConcreteType { get; }
    public List<Type> AllowedInjectionTypes { get; }

    public Binding(Type concreteType, List<Type> allowedInjectionTypes)
    {
        ConcreteType = concreteType;
        AllowedInjectionTypes = allowedInjectionTypes;
    }
}