namespace Injact;

public partial class Guard
{
    public partial class Against
    {
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