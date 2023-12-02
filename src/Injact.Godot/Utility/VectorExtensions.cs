using NativeVector = Injact.Vector3;
using EngineVector = Godot.Vector3;

namespace Injact.Godot;

public static class VectorExtensions
{
    public static NativeVector ToNative(this EngineVector vector)
    {
        return new NativeVector(vector.X, vector.Y, vector.Z);
    }

    public static EngineVector ToEngine(this NativeVector vector)
    {
        return new EngineVector(vector.X, vector.Y, vector.Z);
    }
}