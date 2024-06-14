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

        public static void CircularDependency(Instances instances, Type requestedType)
        {
            var rootParameters = ReflectionHelper.GetParameters(requestedType);

            foreach (var parameter in rootParameters)
            {
                //If there is already an instance created we can assume that the dependency is not circular for that parameter
                if (instances.ContainsKey(parameter.ParameterType))
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
