namespace Injact;

public partial class Guard
{
    public partial class Against
    {
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
    }
}