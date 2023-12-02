namespace Injact.Godot;

//Note: This code is duplicated in NodeWrapper, I decided the complexity of trying to move it to a shared place wasn't worth it
//These classes cannot share a base class as they inherit from different Godot classes
public abstract partial class NodeWrapper3D<T> : Node3D where T : class, IPhysicalConsumer, new()
{
    [Inject] private readonly Injector _injector = null!;
    [Inject] private readonly EditorValueMapper _editorValueMapper = null!;
    
    public T Value { get; set; } = null!;

    public event Action<double> OnUpdate;

    public override void _EnterTree()
    {
        Value = new T();

        Guard.Against.Null(Value, $"Failed to initialise wrapped class on {this}!");

        _injector.InjectInto(Value);

        if (typeof(T).GetMethod("Update")?.DeclaringType == typeof(T))
            OnUpdate += Value.Update;

        _editorValueMapper.Map(this, Value);

        Value.PhysicalProvider = new PhysicalProvider3D(this);
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
        OnUpdate?.Invoke(delta);
        base._Process(delta);
    }
}