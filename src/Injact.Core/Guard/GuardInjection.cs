﻿namespace Injact;

public static partial class Guard
{
    public static partial class Against
    {
        //TODO: This does not appear to be working
        public static void CircularDependency(Bindings bindings, Instances instances, Type requestedType)
        {
            var rootParameters = ReflectionHelper.GetParameters(requestedType);

            foreach (var parameter in rootParameters)
            {
                //If there is already an instance created we can assume that the dependency is not circular for that parameter
                if (instances.ContainsKey(parameter.ParameterType))
                {
                    continue;
                }
                
                bindings.TryGetValue(parameter.ParameterType, out var parameterBinding);
                if (parameterBinding == null)
                {
                    continue;
                }
                
                var parameters = ReflectionHelper.GetParameters(parameterBinding.ConcreteType);
                if (parameters.Any(s => s.ParameterType == requestedType))
                {
                    throw new DependencyException($"Requested type of {parameter.ParameterType} contains a circular dependency!");
                }
            }
        }

        public static void IllegalInjection(Bindings bindings, Type requestedType, Type? requestingType)
        {
            if (requestingType == null)
            {
                return;
            }

            bindings.TryGetValue(requestedType, out var binding);

            if (binding == null || !binding.AllowedInjectionTypes.Any())
            {
                return;
            }

            var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
            if (!isAllowed)
            {
                throw new DependencyException($"{requestingType} requested type of {requestedType} when it is not allowed to!");
            }
        }
    }
}