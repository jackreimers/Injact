namespace Injact;

public static partial class Guard
{
    public static partial class Against
    {
        public static int Negative(int value)
        {
            if (value < 0)
            {
                throw new ArithmeticException("Value must be greater than or equal to zero!");
            }
            
            return value;
        }
        
        public static int ZeroOrNegative(int value)
        {
            if (value <= 0)
            {
                throw new ArithmeticException("Value must be greater than zero!");
            }
            
            return value;
        }
    }
}