namespace Injact;

public abstract class LifecycleObject : ILifecycleObject
{
    private readonly bool _shouldRunUpdate = false;
    private bool _enabled;

    public bool Enabled
    {
        get => _enabled;
        set => OnEnabledSet(value);
    }

    public event Action? OnEnableEvent;
    public event Action? OnDisableEvent;
    public event Action? OnDestroyEvent;

    public virtual void Awake() { }

    public virtual void Start() { }

    public virtual void Update() { }

    public virtual void Destroy()
    {
        Time.OnUpdateEvent -= Update;
        OnDestroyEvent?.Invoke();
    }

    public void Enable()
    {
        Enabled = true;
    }

    public void Disable()
    {
        Enabled = false;
    }

    protected virtual void OnEnabledSet(bool value)
    {
        OnEnabledSet(value, out _);
    }

    protected void OnEnabledSet(bool value, out bool updated)
    {
        if (_enabled == value)
        {
            updated = false;
            return;
        }

        _enabled = value;

        if (_enabled)
        {
            OnEnableEvent?.Invoke();

            if (_shouldRunUpdate)
            {
                Time.OnUpdateEvent += Update;
            }
        }

        else
        {
            OnDisableEvent?.Invoke();
            Time.OnUpdateEvent -= Update;
        }

        updated = true;
    }
}
