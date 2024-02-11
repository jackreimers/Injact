namespace Injact;

public partial class Guard
{
    public partial class Against
    {
        public static void Unassignable<T1, T2>()
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new DependencyException($"Type {typeof(T2)} is not assignable to {typeof(T1)} when it should be!");
        }

        public static void Unassignable(Type type1, Type type2)
        {
            if (!type1.IsAssignableFrom(type2))
                throw new DependencyException($"Type {type2} is not assignable to {type1} when it should be!");
        }
        public static void Unassignable<T1, T2>(string message)
        {
            if (!typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new DependencyException(message);
        }

        public static void Unassignable(Type type1, Type type2, string message)
        {
            if (!type1.IsAssignableFrom(type2))
                throw new DependencyException(message);
        }

        public static void Assignable<T1, T2>()
        {
            if (typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new DependencyException($"Type {typeof(T2)} is assignable to {typeof(T1)} when it shouldn't be!");
        }

        public static void Assignable<T1, T2>(string message)
        {
            if (typeof(T1).IsAssignableFrom(typeof(T2)))
                throw new DependencyException(message);
        }

        public static void Assignable(Type type1, Type type2)
        {
            if (type1.IsAssignableFrom(type2))
                throw new DependencyException($"Type {type2} is assignable to {type1} when it shouldn't be!");
        }

        public static void Assignable(Type type1, Type type2, string message)
        {
            if (type1.IsAssignableFrom(type2))
                throw new DependencyException(message);
        }
    }
}