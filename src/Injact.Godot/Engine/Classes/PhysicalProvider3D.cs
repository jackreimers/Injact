using Godot;
using Injact.Engine;
using Injact.Godot.Utility;
using Injact.Injection;

namespace Injact.Godot.Engine;

public class PhysicalProvider3D : IPhysicalProvider
{
    private readonly Node3D _node;
    
    public PhysicalProvider3D(Node3D node)
    {
        Guard.Against.Null(node, $"Node cannot be null on {this}!");
        _node = node;
    }

    public System.Numerics.Vector3 Position 
        => _node.Position.ToSystem();
    
    public void Translate(System.Numerics.Vector3 translation)
    {
        _node.Translate(translation.ToGodot());
    }

    public void TranslateLocal(System.Numerics.Vector3 translation)
    {
        _node.TranslateObjectLocal(translation.ToGodot());
    }

    public void Rotate(System.Numerics.Vector3 rotation)
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