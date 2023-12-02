namespace Injact.Godot;

public class PhysicalProvider3D : IPhysicalProvider
{
    private readonly Node3D _node;

    public PhysicalProvider3D(Node3D node)
    {
        Guard.Against.Null(node, $"Node cannot be null on {this}!");
        _node = node;
    }

    public Vector3 Position => _node.Position.ToNative();

    public void Translate(Vector3 translation)
    {
        _node.Translate(translation.ToEngine());
    }

    public void TranslateLocal(Vector3 translation)
    {
        _node.TranslateObjectLocal(translation.ToEngine());
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