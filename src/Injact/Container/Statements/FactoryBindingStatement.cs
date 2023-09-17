using System;
using System.Collections.Generic;

namespace Injact;

public class FactoryBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; }
    public Type ConcreteType { get; set; }
    public Type ObjectType { get; set; }

    public List<Type> AllowedInjectionTypes { get; } = new();

    public BindingFlags Flags { get; set; }
    public BindingType BindingType { get; set; }
}

public static class FactoryBindingStatementExtensions { }