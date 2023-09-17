using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Injact.Internal;

public static class Assert
{
    public static void IsNotNull(object obj, string message)
    {
        if (obj == null)
            throw new NullReferenceException(message);
    }

    public static void IsNotNullOrWhitespace(string value, string message)
    {
        if (value == null)
            throw new NullReferenceException(message);
    }

    public static void IsAssignable<TInterface, TConcrete>()
    {
        if (!typeof(TInterface).IsAssignableFrom(typeof(TConcrete)))
            throw new DependencyException($"Type {typeof(TConcrete)} is not assignable to {typeof(TInterface)} when it should be!");
    }

    public static void IsAssignable(Type interfaceType, Type concreteType)
    {
        if (!interfaceType.IsAssignableFrom(concreteType))
            throw new DependencyException($"Type {concreteType} is not assignable to {interfaceType} when it should be!");
    }

    public static void IsNotAssignable<TInterface, TConcrete>()
    {
        if (typeof(TInterface).IsAssignableFrom(typeof(TConcrete)))
            throw new DependencyException($"Type {typeof(TConcrete)} is assignable to {typeof(TInterface)} when it shouldn't be!");
    }

    public static void IsNotAssignable(Type interfaceType, Type concreteType)
    {
        if (interfaceType.IsAssignableFrom(concreteType))
            throw new DependencyException($"Type {concreteType} is assignable to {interfaceType} when it shouldn't be!");
    }

    public static void IsExistingBinding(Bindings bindings, Type type)
    {
        if (!bindings.ContainsKey(type))
            throw new DependencyException($"Requested type of {type} has not been bound!");
    }

    public static void IsNotExistingBinding(Bindings bindings, Type type)
    {
        if (bindings.ContainsKey(type))
            throw new DependencyException($"Binding of type {type} already exists!");
    }

    public static void IsNotCircular(Bindings bindings, Binding binding, IEnumerable<ParameterInfo> rootParameters)
    {
        foreach (var parameter in rootParameters)
        {
            var isRelevant = bindings.TryGetValue(parameter.ParameterType, out var parameterBinding);
            if (!isRelevant)
                continue;

            var parameters = ReflectionHelpers.GetParameters(parameterBinding.ConcreteType);
            if (parameters.Any(s => s.ParameterType == binding.ConcreteType))
                throw new DependencyException($"Requested type of {parameter.ParameterType} contains a circular dependency!");
        }
    }

    public static void IsLegalInjection(Bindings bindings, Type requestedType, Type requestingType)
    {
        if (requestingType == null)
            return;

        bindings.TryGetValue(requestedType, out var binding);

        if (binding == null || !binding.AllowedInjectionTypes.Any())
            return;

        var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
        if (!isAllowed)
            throw new DependencyException($"{requestingType} requested type of {requestedType} when it is not allowed to!");
    }

    public static void IsValidBindingStatement(IBindingStatement bindingStatement)
    {
        switch (bindingStatement.BindingType)
        {
            case BindingType.Object:
                IsValidObjectBindingStatement(bindingStatement as ObjectBindingStatement);
                break;

            case BindingType.Factory:
                IsValidFactoryBindingStatement(bindingStatement as FactoryBindingStatement);
                break;

            default: throw new ArgumentOutOfRangeException();
        }
    }

    public static void IsValidObjectBindingStatement(ObjectBindingStatement bindingStatement)
    {
        IsNotNull(bindingStatement, "Binding statement cannot be null!");

        if (bindingStatement.Flags.HasFlag(BindingFlags.Singleton) && bindingStatement.Instance == null)
            throw new DependencyException($"{bindingStatement.InterfaceType} is marked as a singleton but has no instance provided!");
    }

    public static void IsValidFactoryBindingStatement(FactoryBindingStatement bindingStatement)
    {
        IsNotNull(bindingStatement, "Binding statement cannot be null!");

        if (bindingStatement.ObjectType == null)
            throw new DependencyException($"{nameof(bindingStatement.ObjectType)} is null on {bindingStatement.InterfaceType} factory binding!");
    }
}