using System;
using System.Collections.Generic;
using Godot;
using Injact.Internal;

namespace Injact;

public class ObjectBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; }
    public Type ConcreteType { get; set; }
    public List<Type> AllowedInjectionTypes { get; } = new();

    public object Instance { get; set; }

    public BindingFlags BindingFlags { get; set; }
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
        Assert.IsNotNull(instance, $"Null instance reference on {binding.InterfaceType.Name} binding!");
            
        binding.BindingFlags |= BindingFlags.Singleton;
        binding.Instance = instance;
        return binding;
    }

    public static ObjectBindingStatement FromNode(this ObjectBindingStatement binding, Node node)
    {
        Assert.IsNotNull(node, $"Null node reference on {binding.InterfaceType.Name} binding!");
            
        binding.BindingFlags |= BindingFlags.Singleton;
        binding.Instance = node;
        return binding;
    }
    
    public static ObjectBindingStatement AsSingleton(this ObjectBindingStatement binding)
    {
        binding.BindingFlags |= BindingFlags.Singleton;
        return binding;
    }

    public static ObjectBindingStatement AsTransient(this ObjectBindingStatement binding)
    {
        binding.BindingFlags &= ~BindingFlags.Singleton;
        return binding;
    }

    public static ObjectBindingStatement Immediate(this ObjectBindingStatement binding)
    {
        binding.BindingFlags |= BindingFlags.Immediate;
        return binding;
    }

    public static ObjectBindingStatement Delayed(this ObjectBindingStatement binding)
    {
        binding.BindingFlags &= ~BindingFlags.Immediate;
        return binding;
    }
}