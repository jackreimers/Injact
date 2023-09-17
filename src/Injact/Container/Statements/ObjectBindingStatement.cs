using System;
using System.Collections.Generic;
using Injact.Internal;

namespace Injact
{
    public class ObjectBindingStatement : IBindingStatement
    {
        public Type InterfaceType { get; set; }
        public Type ConcreteType { get; set; }
        public List<Type> AllowedInjectionTypes { get; } = new();

        public object Instance { get; set; }

        public BindingType BindingType { get; set; }
        public BindingFlags Flags { get; set; }
    }

    public static class PendingObjectBindingExtensions
    {
        public static ObjectBindingStatement FromInstance(this ObjectBindingStatement binding, object instance)
        {
            Assert.IsNotNull(instance, $"Null instance reference on {binding.InterfaceType.Name} binding!");

            binding.Instance = instance;
            return binding;
        }

        public static ObjectBindingStatement AsSingleton(this ObjectBindingStatement binding)
        {
            binding.Flags |= BindingFlags.Singleton;
            return binding;
        }

        public static ObjectBindingStatement AsTransient(this ObjectBindingStatement binding)
        {
            binding.Flags &= ~BindingFlags.Singleton;
            return binding;
        }

        public static ObjectBindingStatement Immediate(this ObjectBindingStatement binding)
        {
            binding.Flags |= BindingFlags.Immediate;
            return binding;
        }

        public static ObjectBindingStatement Delayed(this ObjectBindingStatement binding)
        {
            binding.Flags &= ~BindingFlags.Immediate;
            return binding;
        }
    }
}