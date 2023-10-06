using System;
using Godot;
using Injact.Engine;
using Vector3 = System.Numerics.Vector3;

namespace Injact.Godot;

//TODO: Currently any wrapped class will need to be registered with the container which I don't think will be practical.
//TODO: Investigate ways to fix this, perhaps project specific code could be added to the container? Or once the wrapping method has been tested, integrate it into the container.
//TODO: Lots of duplicated code with NodeWrapper, is there a way around this? (interface method declarations?)
public abstract partial class Node3DWrapper<T> : Node3D, IPhysicalProvider where T : class, IPhysicalConsumer
{
    [Inject] protected readonly T value = null!;
    [Inject] protected readonly ILogger _logger = null!;
    
    [Inject] private readonly EditorValueMapper _editorValueMapper = null!;

    public event Action<double> Update;

    public override void _EnterTree()
    {
        Assert.IsNotNull(value, "Wrapped class cannot be null!");

        if (typeof(T).GetMethod("Update")?.DeclaringType == typeof(T))
            Update += value.Update;

        _editorValueMapper.Map(this, value);

        value.PhysicalProvider = this;
        value.Awake();
        base._EnterTree();
    }

    public override void _Ready()
    {
        value.Start();
        base._Ready();
    }

    public override void _Process(double delta)
    {
        Update?.Invoke(delta);
        base._Process(delta);
    }

    public void Translate(Vector3 translation)
    {
        //Translate(new Godot.Vector3(translation.X, translation.Y, translation.Z));
    }

    public void TranslateLocal(Vector3 translation)
    {
        //TranslateObjectLocal(new Godot.Vector3(translation.X, translation.Y, translation.Z));
    }

    public void Rotate(Vector3 rotation) { }
}