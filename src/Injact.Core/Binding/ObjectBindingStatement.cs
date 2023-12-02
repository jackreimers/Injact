namespace Injact;

public class ObjectBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; } = null!;
    public Type ConcreteType { get; set; } = null!;
    public List<Type> AllowedInjectionTypes { get; } = new();

    public object? Instance { get; set; }

    public StatementFlags Flags { get; set; }
}