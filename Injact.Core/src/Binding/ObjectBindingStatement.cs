using System;
using System.Collections.Generic;

namespace Injact;

public class ObjectBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; } = null!;
    public Type ConcreteType { get; set; } = null!;
    public List<Type> AllowedInjectionTypes { get; } = new();

    public object? Instance { get; set; }

    public StatementFlags Flags { get; set; }
}

public static class ObjectBindingExtensions
{
    public static ObjectBindingStatement WhenInjectedInto<TValue>(this ObjectBindingStatement binding)
    {
        binding.AllowedInjectionTypes.Add(typeof(TValue));
        return binding;
    }

    public static ObjectBindingStatement WhenInjectedInto(this ObjectBindingStatement binding, Type allowedType)
    {
        binding.AllowedInjectionTypes.Add(allowedType);
        return binding;
    }
    
    public static ObjectBindingStatement FromInstance(this ObjectBindingStatement binding, object instance)
    {
        Guard.Against.Null(instance, $"Null instance reference on {binding.InterfaceType.Name} binding!");
            
        binding.Flags |= StatementFlags.Singleton;
        binding.Instance = instance;
        return binding;
    }
    
    public static ObjectBindingStatement AsSingleton(this ObjectBindingStatement binding)
    {
        binding.Flags |= StatementFlags.Singleton;
        return binding;
    }

    public static ObjectBindingStatement AsTransient(this ObjectBindingStatement binding)
    {
        binding.Flags &= ~StatementFlags.Singleton;
        return binding;
    }

    public static ObjectBindingStatement Immediate(this ObjectBindingStatement binding)
    {
        binding.Flags |= StatementFlags.Immediate;
        return binding;
    }

    public static ObjectBindingStatement Delayed(this ObjectBindingStatement binding)
    {
        binding.Flags &= ~StatementFlags.Immediate;
        return binding;
    }
}