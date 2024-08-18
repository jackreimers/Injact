namespace Injact;

public interface ILifecycleObject
{
    public bool Enabled { get; set; }

    public event Action? OnEnableEvent;
    public event Action? OnDisableEvent;
    public event Action? OnDestroyEvent;

    public void Awake();

    public void Start();

    public void Update();

    public void Destroy();

    public void Enable();

    public void Disable();
}
