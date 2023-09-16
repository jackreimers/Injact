using System;
using System.Collections.Generic;

namespace Injact
{
    public interface IBindingStatement
    {
        public Type InterfaceType { get; set; }
        public Type ConcreteType { get; set; }

        public List<Type> AllowedInjectionTypes { get; }

        public BindingFlags Flags { get; set; }
        public BindingType BindingType { get; set; }
    }

    public static class IBindingStatementExtensions
    {
        public static IBindingStatement WhenInjectedInto<T>(this IBindingStatement binding)
        {
            binding.AllowedInjectionTypes.Add(typeof(T));
            return binding;
        }

        public static IBindingStatement WhenInjectedInto(this IBindingStatement binding, Type allowedType)
        {
            binding.AllowedInjectionTypes.Add(allowedType);
            return binding;
        }
    }
}