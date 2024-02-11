// ReSharper disable ArrangeObjectCreationWhenTypeNotEvident

namespace Injact.Godot;

public class PhysicalProvider3D : IPhysicalProvider
{
    private readonly Node3D _node;

    public PhysicalProvider3D(Node3D node)
    {
        _node = node;
    }

    public Vector3 Position => _node.Position.ToNative();

    public void Translate(Vector3 translation)
    {
        _node.Translate(translation.ToEngine());
    }

    public void Translate(float x, float y, float z)
    {
        _node.Translate(new(x, y, z));
    }

    public void TranslateLocal(Vector3 translation)
    {
        _node.TranslateObjectLocal(translation.ToEngine());
    }

    public void TranslateLocal(float x, float y, float z)
    {
        _node.TranslateObjectLocal(new(x, y, z));
    }

    public void Rotate(Vector3 rotation)
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
}