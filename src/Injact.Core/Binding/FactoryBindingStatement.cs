namespace Injact;

public class FactoryBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; } = null!;
    public Type ConcreteType { get; set; } = null!;
    public Type ObjectType { get; set; } = null!;

    public List<Type> AllowedInjectionTypes { get; } = new();

    public StatementFlags Flags { get; set; }
}