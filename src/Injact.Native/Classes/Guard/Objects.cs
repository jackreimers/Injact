using System;

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

        public static void Null(object obj, string message)
        {
            if (obj == null)
                throw new NullReferenceException(message);
        }

        public static void NotNull(object obj, string message)
        {
            if (obj != null)
                throw new ArgumentException(message);
        }

        public static void NullOrWhitespace(string value, string message)
        {
            if (value == null)
                throw new NullReferenceException(message);
        }
    }
}