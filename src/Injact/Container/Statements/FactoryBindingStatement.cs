using System;
using System.Collections.Generic;
using Injact.Internal;

namespace Injact
{
    public class FactoryBindingStatement : IBindingStatement
    {
        public Type InterfaceType { get; set; }
        public Type ConcreteType { get; set; }
        public Type ObjectType { get; set; }

        public List<Type> AllowedInjectionTypes { get; } = new();

        public BindingFlags Flags { get; set; }
        public BindingType BindingType { get; set; }
    }

    public static class FactoryBindingStatementExtensions
    {
        public static FactoryBindingStatement ForType<TInterface>(this FactoryBindingStatement binding)
        {
            Assert.IsNotNull(typeof(TInterface), $"Cannot bind factory for null type on {binding.InterfaceType.Name} binding!");

            binding.ObjectType = typeof(TInterface);
            return binding;
        }

        public static FactoryBindingStatement ForType(this FactoryBindingStatement binding, Type type)
        {
            Assert.IsNotNull(type, $"Cannot bind factory for null type on {binding.InterfaceType.Name} binding!");

            binding.ObjectType = type;
            return binding;
        }
    }
}