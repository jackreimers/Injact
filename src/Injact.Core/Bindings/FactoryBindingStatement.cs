using System;
using System.Collections.Generic;

namespace Injact;

public class FactoryBindingStatement : IBindingStatement
{
    public Type InterfaceType { get; set; }
    public Type ConcreteType { get; set; }
    public Type ObjectType { get; set; }

    public List<Type> AllowedInjectionTypes { get; } = new();

    public StatementFlags Flags { get; set; }
}

public static class FactoryBindingStatementExtensions { }