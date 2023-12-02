namespace Injact;

public interface IBindingStatement
{
    public Type InterfaceType { get; set; }
    public Type ConcreteType { get; set; }

    public List<Type> AllowedInjectionTypes { get; }

    public StatementFlags Flags { get; set; }
}

public static class IBindingStatementExtensions
{
    public static IBindingStatement WhenInjectedInto<TValue>(this IBindingStatement binding)
    {
        binding.AllowedInjectionTypes.Add(typeof(TValue));
        return binding;
    }

    public static IBindingStatement WhenInjectedInto(this IBindingStatement binding, Type allowedType)
    {
        binding.AllowedInjectionTypes.Add(allowedType);
        return binding;
    }
}