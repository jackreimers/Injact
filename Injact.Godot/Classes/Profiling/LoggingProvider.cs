using System;

namespace Injact.Godot;

public class LoggingProvider : ILoggingProvider
{
    public ILogger GetLogger<T>()
    {
        return new Logger<T>();
    }

    public Type GetLoggerType()
    {
        return typeof(Logger<>);
    }
}