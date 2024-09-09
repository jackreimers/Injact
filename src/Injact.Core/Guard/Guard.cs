namespace Injact.Validation;

public static class Guard
{
    public static class Against
    {
        private const string ObjectNullErrorMessage = "Object \"{0}\" cannot be null.";
        private const string ObjectNotNullErrorMessage = "Object \"{0}\" must be null.";
        private const string StringNullOrEmptyErrorMessage = "String cannot be null or empty.";
        private const string StringNullOrWhitespaceErrorMessage = "String cannot be null or whitespace.";
        private const string IllegalInjectionErrorMessage = "\"{0}\" requested type of \"{1}\" when it is not allowed to.";
        private const string CircularInjectionErrorMessage = "Requested type of {0} contains a circular dependency.";

        public static void Condition(bool condition, string message)
        {
            if (condition)
            {
                throw new DependencyException(message);
            }
        }

        public static void Assignable<T1, T2>(string message)
        {
            if (typeof(T1).IsAssignableFrom(typeof(T2)))
            {
                throw new DependencyException(message);
            }
        }

        public static T Null<T>(T? value)
        {
            if (value == null || value == null!)
            {
                throw new NullReferenceException(string.Format(ObjectNullErrorMessage, typeof(T).Name));
            }

            return value;
        }

        public static T? NotNull<T>(T? value)
        {
            if (value != null || value != null!)
            {
                throw new ArgumentException(string.Format(ObjectNotNullErrorMessage, typeof(T).Name));
            }

            return value;
        }

        public static string NullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new NullReferenceException(StringNullOrEmptyErrorMessage);
            }

            return value;
        }

        public static string NullOrWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NullReferenceException(StringNullOrWhitespaceErrorMessage);
            }

            return value;
        }

        public static int Negative(int value)
        {
            if (value < 0)
            {
                throw new ArithmeticException("Value must be greater than or equal to zero!");
            }

            return value;
        }

        public static int ZeroOrNegative(int value)
        {
            if (value <= 0)
            {
                throw new ArithmeticException("Value must be greater than zero!");
            }

            return value;
        }

        public static void IllegalInjection(ObjectBinding binding, Type? requestingType)
        {
            if (requestingType == null)
            {
                return;
            }

            if (!binding.AllowedInjectionTypes.Any())
            {
                return;
            }

            var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
            if (!isAllowed)
            {
                throw new DependencyException(string.Format(IllegalInjectionErrorMessage, requestingType, binding.InterfaceType));
            }
        }

        public static void IllegalInjection(FactoryBinding binding, Type? requestingType)
        {
            if (requestingType == null)
            {
                return;
            }

            if (!binding.AllowedInjectionTypes.Any())
            {
                return;
            }

            var isAllowed = binding.AllowedInjectionTypes.Any(allowed => allowed.IsAssignableFrom(requestingType));
            if (!isAllowed)
            {
                throw new DependencyException(string.Format(IllegalInjectionErrorMessage, requestingType, binding.InterfaceType));
            }
        }

        public static void CircularInjection(ContainerOptions containerOptions, ObjectBindings instances, Type requestedType, object[] arguments)
        {
            //TODO: This is not currently checking factory bindings
            var rootParameters = ReflectionHelpers.GetConstructorParameters(requestedType);
            var argTypes = arguments
                .Select(arg => arg.GetType())
                .ToArray();

            foreach (var parameter in rootParameters)
            {
                //If there is already an instance created or an argument passed then we can assume that the dependency is not circular for that parameter
                if (instances.ContainsKey(parameter.ParameterType) || argTypes.Contains(parameter.ParameterType))
                {
                    continue;
                }

                //If the parameter has a default value and we are not injecting into default properties then we can assume that the dependency is not circular for that parameter
                if (!containerOptions.InjectIntoDefaultProperties && parameter.HasDefaultValue)
                {
                    continue;
                }

                if (ReflectionHelpers.HasTypeInDependencyTree(parameter.ParameterType, requestedType))
                {
                    throw new DependencyException(string.Format(CircularInjectionErrorMessage, parameter.ParameterType));
                }
            }
        }
    }
}