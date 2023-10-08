using System;
using Godot;
using Injact.Engine;
using Injact.Injection;

namespace Injact.Godot.Engine;

//Note: This code is duplicated in NodeWrapper, I decided the complexity of trying to move it to a shared place wasn't worth it
//These classes cannot share a base class as they inherit from different Godot classes
public abstract partial class NodeWrapper3D<T> : Node3D, IPhysicalProvider where T : class, IPhysicalConsumer, new()
{
    [Inject] private readonly Injector _injector = null!;
    [Inject] private readonly EditorValueMapper _editorValueMapper = null!;

    public T Value { get; set; } = null!;

    public event Action<double> Update;

    public override void _EnterTree()
    {
        Value = new T();

        Assert.IsNotNull(Value, $"Failed to initialise wrapped class on {this}!");

        _injector.InjectInto(Value);

        if (typeof(T).GetMethod("Update")?.DeclaringType == typeof(T))
            Update += Value.Update;

        _editorValueMapper.Map(this, Value);

        Value.PhysicalProvider = this;
        Value.Awake();
        base._EnterTree();
    }

    public override void _Ready()
    {
        Value.Start();
        base._Ready();
    }

    public override void _Process(double delta)
    {
        Update?.Invoke(delta);
        base._Process(delta);
    }

    public void Translate(System.Numerics.Vector3 translation)
    {
        Translate(new Vector3(translation.X, translation.Y, translation.Z));
    }

    public void TranslateLocal(System.Numerics.Vector3 translation)
    {
        TranslateObjectLocal(new Vector3(translation.X, translation.Y, translation.Z));
    }

    public void Rotate(System.Numerics.Vector3 rotation)
    {
        RotateX(rotation.X);
        RotateY(rotation.Y);
        RotateZ(rotation.Z);
    }

    public void Rotate(float x, float y, float z)
    {
        RotateX(x);
        RotateY(y);
        RotateZ(z);
    }
}