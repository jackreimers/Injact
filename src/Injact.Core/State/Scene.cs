namespace Injact;

public static class Scene
{
    public static event Action? OnUpdate;

    public static void TriggerUpdate()
    {
        OnUpdate?.Invoke();
    }
}