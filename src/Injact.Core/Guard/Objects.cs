namespace Injact;

public partial class Guard
{
    public partial class Against
    {
        public static void Condition(bool condition, string message)
        {
            if (condition)
                throw new NotSupportedException(message);
        }

        public static void Null(object? obj, string message)
        {
            if (obj == null)
                throw new NullReferenceException(message);
        }

        public static void NotNull(object? obj, string message)
        {
            if (obj != null)
                throw new ArgumentException(message);
        }

        public static void NullOrWhitespace(string value, string message)
        {
            if (value == null)
                throw new NullReferenceException(message);
        }
        
        //TODO: Putting this here for now but not sure it belongs here
        public static void NotObserved<T>(IEnumerable<T> observed, T target)
        {
            if (!observed.Contains(target))
                throw new InvalidOperationException($"Received an update from {target} when it was not being observed!");
        }
    }
}