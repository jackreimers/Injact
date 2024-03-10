namespace Injact;

public static partial class Guard
{
    public static partial class Against
    {
        public static void Condition(bool condition, string message)
        {
            if (condition)
            {
                throw new NotSupportedException(message);
            }
        }

        public static T Null<T>(T? value)
        {
            if (value == null || value == null!)
            {
                throw new NullReferenceException($"Expected {nameof(T)} to be non-null!");
            }

            return value;
        }

        public static T? NotNull<T>(T? value)
        {
            if (value != null || value != null!)
            {
                throw new ArgumentException($"Expected {nameof(T)} to be null!");
            }

            return value;
        }

        public static string NullOrEmpty(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new NullReferenceException("String cannot be empty or null!");
            }

            return value;
        }

        public static string NullOrWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new NullReferenceException("String cannot be empty, null or whitespace!");
            }

            return value;
        }
    }
}