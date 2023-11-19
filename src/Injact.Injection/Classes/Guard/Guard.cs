using System;
using System.Linq;
using Injact.Utility;

namespace Injact.Injection;

public class Guard
{
    public class Against
    {
        public static void Condition(bool condition, string message)
        {
            if (condition)
                throw new NotSupportedException(message);
        }
        
        public static void Null(object obj, string message)
        {
            if (obj == null)
                throw new NullReferenceException(message);
        }
        
        public static void NotNull(object obj, string message)
        {
            if (obj != null)
                throw new ArgumentException(message);
        }

        public static void NullOrWhitespace(string value, string message)
        {
            if (value == null)
                throw new NullReferenceException(message);
        }

        public static void Unassignable<TInterface, TConcrete>()
        {
            if (!typeof(TInterface).IsAssignableFrom(typeof(TConcrete)))
                throw new DependencyException($"Type {typeof(TConcrete)} is not assignable to {typeof(TInterface)} when it should be!");
        }

        public static void Unassignable(Type interfaceType, Type concreteType)
        {
            if (!interfaceType.IsAssignableFrom(concreteType))
                throw new DependencyException($"Type {concreteType} is not assignable to {interfaceType} when it should be!");
        }

        public static void Assignable<TInterface, TConcrete>()
        {
            if (typeof(TInterface).IsAssignableFrom(typeof(TConcrete)))
                throw new DependencyException($"Type {typeof(TConcrete)} is assignable to {typeof(TInterface)} when it shouldn't be!");
        }

        public static void Assignable(Type interfaceType, Type concreteType)
        {
            if (interfaceType.IsAssignableFrom(concreteType))
                throw new DependencyException($"Type {concreteType} is assignable to {interfaceType} when it shouldn't be!");
        }

        public static void MissingBinding(Bindings bindings, Type type)
        {
            if (!bindings.ContainsKey(type))
                throw new DependencyException($"Requested type of {type} has not been bound!");
        }

        public static void ExistingBinding(Bindings bindings, Type type)
        {
            if (bindings.ContainsKey(type))
                throw new DependencyException($"Binding of type {type} already exists!");
        }

        public static void CircularDependency(Bindings bindings, Type requestedType)
        {
            var rootParameters = ReflectionHelpers.GetParameters(requestedType);
            
            foreach (var parameter in rootParameters)
            {
                var isRelevant = bindings.TryGetValue(parameter.ParameterType, out var parameterBinding);
                if (!isRelevant)
                    continue;

                var parameters = ReflectionHelpers.GetParameters(parameterBinding.ConcreteType);
                if (parameters.Any(s => s.ParameterType == requestedType))
                    throw new DependencyException($"Requested type of {parameter.ParameterType} contains a circular dependency!");
            }
        }

        public static void IllegalInjection(Bindings bindings, Type requestedType, Type requestingType)
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

        public static void InvalidBindingStatement(IBindingStatement bindingStatement)
        {
            if (bindingStatement.Flags.HasFlag(StatementFlags.Factory))
                InvalidFactoryBindingStatement(bindingStatement as FactoryBindingStatement);

            else
                InvalidObjectBindingStatement(bindingStatement as ObjectBindingStatement);
        }

        public static void InvalidObjectBindingStatement(ObjectBindingStatement bindingStatement)
        {
            Null(bindingStatement, "Binding statement cannot be null!");
        }

        public static void InvalidFactoryBindingStatement(FactoryBindingStatement bindingStatement)
        {
            Null(bindingStatement, "Binding statement cannot be null!");

            if (bindingStatement.ObjectType == null)
                throw new DependencyException($"{nameof(bindingStatement.ObjectType)} is null on {bindingStatement.InterfaceType} factory binding!");
        }
    }
}