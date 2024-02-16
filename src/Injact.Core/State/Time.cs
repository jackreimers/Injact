namespace Injact;

public static class Time
{
    public static float Delta { get; set; }

    public static event Action? OnUpdate;

    public static void TriggerUpdate()
    {
        OnUpdate?.Invoke();
    }
}