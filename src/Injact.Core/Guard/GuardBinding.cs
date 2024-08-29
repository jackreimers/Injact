namespace Injact;

public static partial class Guard
{
    public static partial class Against
    {
        public static ObjectBindingStatement InvalidObjectBindingStatement(IBindingStatement bindingStatement)
        {
            //TODO: Implement or remove this
            return (ObjectBindingStatement)bindingStatement ?? throw new DependencyException("Invalid object binding statement!");
        }

        public static ObjectBindingStatement InvalidObjectBindingStatement(ObjectBindingStatement bindingStatement)
        {
            //TODO: Implement or remove this
            return bindingStatement;
        }

        public static FactoryBindingStatement InvalidFactoryBindingStatement(IBindingStatement bindingStatement)
        {
            //TODO: Implement or remove this
            return (FactoryBindingStatement)bindingStatement ?? throw new DependencyException("Invalid factory binding statement!");
        }

        public static FactoryBindingStatement InvalidFactoryBindingStatement(FactoryBindingStatement bindingStatement)
        {
            //TODO: Implement or remove this
            return bindingStatement;
        }

        public static void CircularDependency(ContainerOptions containerOptions, Instances instances, Type requestedType, object[] args)
        {
            var rootParameters = ReflectionHelper.GetParameters(requestedType);
            var argTypes = args
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

                if (ReflectionHelper.HasTypeInDependencyTree(parameter.ParameterType, requestedType))
                {
                    throw new DependencyException($"Requested type of {parameter.ParameterType} contains a circular dependency!");
                }
            }
        }
    }
}
