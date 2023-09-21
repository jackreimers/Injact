﻿namespace Injact.Profiling;

public interface ILogger
{
    public void LogInformation(string message, bool condition = true);
    
    public void LogWarning(string message, bool condition = true);
    
    public void LogError(string message, bool condition = true);
    
    public void LogTrace(string message, object[] args);
}