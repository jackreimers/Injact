using Godot;

namespace Injact.Profiling;

public class GodotLogger : ILogger
{
    private readonly LoggingFlags _loggingFlags;
    
    public GodotLogger(LoggingFlags loggingFlags)
    {
        _loggingFlags = loggingFlags;
    }
    
    public void LogInformation(string message, bool condition = true)
    {
        if (condition && _loggingFlags.HasFlag(LoggingFlags.Information))
            GD.Print($"[Injact.Info] {message}");
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition && _loggingFlags.HasFlag(LoggingFlags.Warning))
            GD.Print($"[Injact.Warn] {message}");
    }

    public void LogError(string message, bool condition = true)
    {
        if (condition)
            GD.Print($"[Injact.Error] {message}");
    }

    public void LogTrace(string message, object[] args)
    {
        GD.Print($"[Injact.Trace] {string.Format(message, args)}");
    }
}