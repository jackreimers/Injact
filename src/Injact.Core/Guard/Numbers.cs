namespace Injact;

public partial class Guard
{
    public partial class Against
    {
        public static void Negative(int value, string message)
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(message);
        }
        
        public static void ZeroOrNegative(int value, string message)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(message);
        }
    }
}