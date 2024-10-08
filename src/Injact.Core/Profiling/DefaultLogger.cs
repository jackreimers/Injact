﻿namespace Injact;

public class DefaultLogger<T> : ILogger
{
    private readonly string _typeName;

    public DefaultLogger()
    {
        var typeName = typeof(T).Name;
        _typeName = typeName is nameof(DiContainer) or nameof(Injector)
            ? "Injact"
            : typeName;

        LogWarning("DefaultLogger is being used, no messages will be logged to the game engine console!");
    }

    public void LogInformation(string message, bool condition = true)
    {
        if (condition)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogWarning(string message, bool condition = true)
    {
        if (condition)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogError(string message, bool condition = true)
    {
        if (condition)
        {
            Console.WriteLine($"[{_typeName}] {message}");
        }
    }

    public void LogTrace(string message, object[] args)
    {
        Console.WriteLine($"[Trace] {string.Format(message, args)}");
    }
}
