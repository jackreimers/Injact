using Godot;

namespace Injact.Godot;

public static class VectorExtensions
{
    public static System.Numerics.Vector3 ToSystem(this Vector3 vector)
    {
        return new System.Numerics.Vector3(vector.X, vector.Y, vector.Z);
    }
    
    public static Vector3 ToGodot(this System.Numerics.Vector3 vector)
    {
        return new Vector3(vector.X, vector.Y, vector.Z);
    }
}