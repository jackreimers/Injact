using System;
using System.Collections.Generic;

namespace Injact.Injection;

public class Binding
{
    public Binding(Type concreteType, List<Type> allowedInjectionTypes)
    {
        ConcreteType = concreteType;
        AllowedInjectionTypes = allowedInjectionTypes;
    }

    public Type ConcreteType { get; }
    public List<Type> AllowedInjectionTypes { get; }
}