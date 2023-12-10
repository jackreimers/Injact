namespace Injact;

public struct Mathf
{
    public const float PI = 3.1415927f;

    public static float Sqrt(float value)
    {
        return (float)Math.Sqrt(value);
    }

    public static float Round(float value)
    {
        return (float)Math.Round(value);
    }

    public static float Round(float value, int digits)
    {
        return (float)Math.Round(value, digits);
    }

    public static float Floor(float value)
    {
        return (float)Math.Floor(value);
    }

    public static float Ceil(float value)
    {
        return (float)Math.Ceiling(value);
    }

    public static float Min(float a, float b)
    {
        return a < b
            ? a
            : b;
    }
    
    public static float Max(float a, float b)
    {
        return a > b
            ? a
            : b;
    }

    public static float Clamp(float value, float min, float max)
    {
        if (value < min)
            value = min;

        else if (value > max)
            value = max;

        return value;
    }

    public static float Clamp01(float value)
    {
        return value switch
        {
            < 0f => 0f,
            > 1f => 1f,
            _ => value
        };
    }
    
    public static float Sin(float value)
    {
        return (float)Math.Sin(value);
    }

    public static float Cos(float value)
    {
        return (float)Math.Cos(value);
    }

    public static float Tan(float value)
    {
        return (float)Math.Tan(value);
    }

    public static float Asin(float value)
    {
        return (float)Math.Asin(value);
    }

    public static float Acos(float value)
    {
        return (float)Math.Acos(value);
    }

    public static float Atan(float value)
    {
        return (float)Math.Atan(value);
    }

    public static float Atan2(float y, float x)
    {
        return (float)Math.Atan2(y, x);
    }
}