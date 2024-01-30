using System.Numerics;

namespace Injact;

public struct Vector3 : IEquatable<Vector3>
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }

    public float Magnitude => GetMagnitude(this);

    public Vector3 Normalised => GetNormalised(this);

    public Vector3()
    {
        X = 0;
        Y = 0;
        Z = 0;
    }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Z);
    }

    public bool Equals(Vector3 other)
    {
        return X.Equals(other.X) && Y.Equals(other.Y) && Z.Equals(other.Z);
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector3 other && Equals(other);
    }

    public static Vector3 operator +(Vector3 first, Vector3 second)
    {
        return new Vector3(first.X + second.X, first.Y + second.Y, first.Z + second.Z);
    }

    public static Vector3 operator -(Vector3 first, Vector3 second)
    {
        return new Vector3(first.X - second.X, first.Y - second.Y, first.Z - second.Z);
    }

    public static Vector3 operator -(Vector3 vector)
    {
        return new Vector3(-vector.X, -vector.Y, -vector.Z);
    }

    public static Vector3 operator *(Vector3 vector, float amount)
    {
        return new Vector3(vector.X * amount, vector.Y * amount, vector.Z * amount);
    }

    public static Vector3 operator *(float amount, Vector3 vector)
    {
        return new Vector3(vector.X * amount, vector.Y * amount, vector.Z * amount);
    }

    public static Vector3 operator /(Vector3 vector, float amount)
    {
        return new Vector3(vector.X / amount, vector.Y / amount, vector.Z / amount);
    }

    public static bool operator ==(Vector3 first, Vector3 second)
    {
        var num1 = first.X - second.X;
        var num2 = first.Y - second.Y;
        var num3 = first.Z - second.Z;

        return num1 * (double)num1 + num2 * (double)num2 + num3 * (double)num3 < 9.999999439624929E-11;
    }

    public static bool operator !=(Vector3 first, Vector3 second)
    {
        return !(first == second);
    }

    private static float GetMagnitude(Vector3 vector)
    {
        return Mathf.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
    }

    private static Vector3 GetNormalised(Vector3 vector)
    {
        var magnitude = GetMagnitude(vector);
        return vector / magnitude; 
    }

    public static float Distance(Vector3 first, Vector3 second)
    {
        return GetMagnitude(first - second);
    }

    public static float Dot(Vector3 first, Vector3 second)
    {
        return (float)(first.X * (double)second.X + first.Y * (double)second.Y + first.Z * (double)second.Z);
    }

    public static Vector3 Cross(Vector3 first, Vector3 second)
    {
        return new Vector3(
            (float)(first.Y * (double)second.Z - first.Z * (double)second.Y),
            (float)(first.Z * (double)second.X - first.X * (double)second.Z),
            (float)(first.X * (double)second.Y - first.Y * (double)second.X));
    }

    public static Vector3 Translate(Vector3 point, Vector3 reference, float distance)
    {
        return point + (point - reference).Normalised * distance;
    }

    public static Vector3 Rotate(Vector3 point, Vector3 axis, float angle)
    {
        angle = angle.ToRadians();
        return point * Mathf.Cos(angle) + Cross(axis, point) * Mathf.Sin(angle) + axis * Dot(axis, point) * (1 - Mathf.Cos(angle));
    }

    public static Vector3 Pivot(Vector3 point, Vector3 pivot, Vector3 axis, float angle)
    {
        return pivot + Rotate(point - pivot, axis, angle);
    }
    
    public static Vector3 Round(Vector3 vector)
    {
        return new Vector3(
            Mathf.Round(vector.X), 
            Mathf.Round(vector.Y), 
            Mathf.Round(vector.Z));
    }
    
    public static Vector3 Round(Vector3 vector, float step)
    {
        var factor = step / 1;
        
        return new Vector3(
            Mathf.Round(vector.X / factor) * factor,
            Mathf.Round(vector.Y / factor) * factor,
            Mathf.Round(vector.Z / factor) * factor);}

    public static Vector3[] GetPointsInCircle(Vector3 origin, float radius, int resolution)
    {
        var points = new Vector3[resolution];

        for (var i = 0; i < resolution; i++)
        {
            //Divide the circle into how ever many slices as per the resolution variable
            double val = Mathf.Round(360 / resolution) * i;

            //Convert to radians
            val *= Math.PI / 180;

            var posxy = new Vector2((float)Math.Sin(val), (float)Math.Cos(val));
            var posxyz = new Vector3(posxy.X, 0, posxy.Y);

            posxyz *= radius;
            points[i] = posxyz + origin;
        }

        return points;
    }

    public static readonly Vector3 Zero = new(0, 0, 0);
    public static readonly Vector3 One = new(1, 1, 1);
    public static readonly Vector3 Forward = new(0, 0, 1);
    public static readonly Vector3 Backward = new(0, 0, -1);
    public static readonly Vector3 Up = new(0, 1, 0);
    public static readonly Vector3 Down = new(0, -1, 0);
    public static readonly Vector3 Left = new(-1, 0, 0);
    public static readonly Vector3 Right = new(1, 0, 0);
}