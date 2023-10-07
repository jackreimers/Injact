using System;
using Godot;
using Injact.Engine;
using Injact.Injection;

namespace Injact.Godot.Engine;

public abstract partial class NodeWrapper<T> : Node, ILifecycleProvider where T : class, ILifecycleConsumer, new()
{
    [Inject] private readonly Injector _injector = null!;
    [Inject] private readonly EditorValueMapper _editorValueMapper = null!;

    public T Value { get; set; } = null!;

    public event Action<double> Update;
    
    public override void _EnterTree()
    {
        //Note: This code is duplicated in NodeWrapper3D, I decided the complexity of trying to move it to a shared place wasn't worth it
        //These classes cannot share a base class as they inherit from different Godot classes
        
        Value = new T();
        
        Assert.IsNotNull(Value, $"Failed to initialise wrapped class on {this}!");
        
        _injector.InjectInto(Value);
        
        if (typeof(T).GetMethod("Update")?.DeclaringType == typeof(T))
            Update += Value.Update;

        _editorValueMapper.Map(this, Value);

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
}