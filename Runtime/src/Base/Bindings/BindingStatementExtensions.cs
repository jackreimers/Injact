using System;
using Injact.Internal;
using UnityEngine;

namespace Injact
{
    public static class BindingStatementExtensions
    {
        public static BindingStatement FromInstance(this BindingStatement binding, object instance)
        {
            Assert.IsNotNull(instance, $"Null instance reference on {binding.InterfaceType.Name} binding!");

            binding.Instance = instance;
            return binding;
        }

        public static BindingStatement FromComponent(this BindingStatement binding, MonoBehaviour component)
        {
            Assert.IsNotNull(component, $"Null component reference on {binding.InterfaceType.Name} binding!");

            binding.Instance = component;
            binding.Flags |= BindingFlags.Singleton;
            
            return binding;
        }

        public static BindingStatement AsSingleton(this BindingStatement binding)
        {
            binding.Flags |= BindingFlags.Singleton;
            return binding;
        }

        public static BindingStatement AsTransient(this BindingStatement binding)
        {
            binding.Flags &= ~BindingFlags.Singleton;
            return binding;
        }

        public static BindingStatement Immediate(this BindingStatement binding)
        {
            binding.Flags |= BindingFlags.Immediate;
            return binding;
        }

        public static BindingStatement NonImmediate(this BindingStatement binding)
        {
            binding.Flags &= ~BindingFlags.Immediate;
            return binding;
        }

        public static BindingStatement WhenInjectedInto<T>(this BindingStatement binding)
        {
            binding.AllowedInjectionTypes.Add(typeof(T));
            return binding;
        }

        public static BindingStatement WhenInjectedInto(this BindingStatement binding, Type allowedType)
        {
            binding.AllowedInjectionTypes.Add(allowedType);
            return binding;
        }
    }
}