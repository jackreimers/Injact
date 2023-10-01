using Godot;

namespace Injact.Profiling;

public class Logger<T> : ILogger
{
    private readonly string _typeName;
    private readonly LoggingFlags _loggingFlags;
    
    public Logger(LoggingFlags loggingFlags)
    {
        _loggingFlags = loggingFlags;
        
        var typeName = typeof(T).Name;
        _typeName = typeName is nameof(DiContainer) or nameof(Injector) or nameof(Context)
            ? "Injact"
            : typeName;
    }

    public void LogInformation(string message, bool condition = true)
    {
        if (condition && _loggingFlags.HasFlag(LoggingFlags.Information))
            GD.Print($"[{_typeName}] {message}");
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition && _loggingFlags.HasFlag(LoggingFlags.Warning))
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