using NativeVector = Injact.Vector3;
using EngineVector = Godot.Vector3;

namespace Injact.Godot;

public class PhysicalProvider3D : IPhysicalProvider
{
    private readonly Node3D _node;

    public PhysicalProvider3D(Node3D node)
    {
        _node = node;
    }

    public NativeVector Position => _node.Position.ToNative();

    public void Translate(NativeVector translation)
    {
        _node.Translate(translation.ToEngine());
    }

    public void Translate(float x, float y, float z)
    {
        _node.Translate(new EngineVector(x, y, z));
    }

    public void TranslateLocal(NativeVector translation)
    {
        _node.TranslateObjectLocal(translation.ToEngine());
    }

    public void TranslateLocal(float x, float y, float z)
    {
        _node.TranslateObjectLocal(new EngineVector(x, y, z));
    }

    public void Rotate(NativeVector rotation)
    {
        _node.RotateX(rotation.X);
        _node.RotateY(rotation.Y);
        _node.RotateZ(rotation.Z);
    }

    public void Rotate(float x, float y, float z)
    {
        _node.RotateX(x);
        _node.RotateY(y);
        _node.RotateZ(z);
    }
    
    public NativeVector LookAt(NativeVector target)
    {
        _node.LookAt(target.ToEngine());
        return _node.Rotation.ToNative();
    }
}