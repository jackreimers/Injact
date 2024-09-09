namespace Injact.Objects;

public abstract class LifecycleObject : ILifecycleObject
{
    //ReSharper disable once ConvertToConstant.Local
    //This will be set to true by the dependency injection container
    private readonly bool _shouldRunUpdate = false;
    private bool _isEnabled;

    public bool IsEnabled
    {
        get => _isEnabled;
        set => OnEnabledChanged(value);
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
        IsEnabled = true;
    }

    public void Disable()
    {
        IsEnabled = false;
    }

    private void OnEnabledChanged(bool value)
    {
        if (_isEnabled == value)
        {
            return;
        }

        _isEnabled = value;

        if (_isEnabled)
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
    }
}
