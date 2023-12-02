﻿using System.Diagnostics;

namespace Injact;

public class Profile : IDisposable
{
    private readonly ILogger _logger;
    private readonly string _message;
    private readonly bool _condition;

    private readonly Stopwatch _stopwatch;

    public Profile(ILogger logger, string message, bool condition = true)
    {
        _logger = logger;
        _message = message;
        _condition = condition;

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
        
        if (_condition)
            _logger.LogTrace(_message, new object[] { _stopwatch.ElapsedMilliseconds + "ms" });
    }
    
    public void Dispose()
    {
        Stop();
    }
}