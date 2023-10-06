using Godot;

namespace Injact.Godot;

public class Logger<T> : ILogger
{
    private readonly string _typeName;
    
    public Logger()
    {
        var typeName = typeof(T).Name;
        _typeName = typeName is nameof(DiContainer) or nameof(Injector) or nameof(Context)
            ? "Injact"
            : typeName;
    }

    public void LogInformation(string message, bool condition = true)
    {
        if (condition)
            GD.Print($"[{_typeName}] {message}");
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition)
            GD.PushWarning($"[{_typeName}] {message}");
    }

    public void LogError(string message, bool condition = true)
    {
        if (condition)
            GD.PushError($"[{_typeName}] {message}");
    }

    public void LogTrace(string message, object[] args)
    {
        GD.Print($"[Trace] {string.Format(message, args)}");
    }
}