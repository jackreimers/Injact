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
    }
}