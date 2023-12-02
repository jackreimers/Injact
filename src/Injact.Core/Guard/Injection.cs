namespace Injact;

public partial class Guard
{
    public partial class Against
    {
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
    }
}