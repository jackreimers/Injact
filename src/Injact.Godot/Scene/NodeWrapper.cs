using System;
using Godot;
using Injact.Engine;

namespace Injact.Godot;

//TODO: Currently any wrapped class will need to be registered with the container which I don't think will be practical.
//TODO: Investigate ways to fix this, perhaps project specific code could be added to the container? Or once the wrapping method has been tested, integrate it into the container.
public abstract partial class NodeWrapper<T> : Node, ILifecycleProvider where T : class, ILifecycleConsumer
{
    [Inject] protected readonly T value = null!;
    [Inject] protected readonly ILogger _logger = null!;
    
    [Inject] private readonly EditorValueMapper _editorValueMapper = null!;

    public event Action<double> Update;
    
    public override void _EnterTree()
    {
        Assert.IsNotNull(value, "Wrapped class cannot be null!");

        //Only call the update method on the wrapped class if it actually implements it
        if (typeof(T).GetMethod("Update")?.DeclaringType == typeof(T))
            Update += value.Update;

        _editorValueMapper.Map(this, value);

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
}