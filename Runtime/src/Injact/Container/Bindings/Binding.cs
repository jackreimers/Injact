using System;
using System.Collections.Generic;

namespace Injact
{
    public class Binding
    {
        public Binding(Type interfaceType, Type concreteType, List<Type> allowedInjectionTypes)
        {
            InterfaceType = interfaceType;
            ConcreteType = concreteType;
            AllowedInjectionTypes = allowedInjectionTypes;
        }

        public Type InterfaceType { get; }
        public Type ConcreteType { get; }
        public List<Type> AllowedInjectionTypes { get; }
    }
}