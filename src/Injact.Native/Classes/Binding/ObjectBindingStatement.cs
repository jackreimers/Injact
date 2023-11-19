using System;
using System.Collections.Generic;
using Godot;

namespace Injact;

public class ObjectBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; }
    public Type ConcreteType { get; set; }
    public List<Type> AllowedInjectionTypes { get; } = new();

    public object Instance { get; set; }

    public StatementFlags Flags { get; set; }
}

public static class PendingObjectBindingExtensions
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

    public static ObjectBindingStatement FromNode(this ObjectBindingStatement binding, Node node)
    {
        Guard.Against.Null(node, $"Null node reference on {binding.InterfaceType.Name} binding!");
            
        binding.Flags |= StatementFlags.Singleton;
        binding.Instance = node;
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