using System;
using System.Collections.Generic;

namespace Injact
{
    public class BindingStatement
    {
        public Type InterfaceType { get; set; }
        public Type ConcreteType { get; set; }
        public List<Type> AllowedInjectionTypes { get; } = new();

        public object Instance { get; set; }
        public BindingFlags Flags { get; set; }
    }
}