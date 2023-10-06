using System.Diagnostics;
using System.Linq;

namespace Injact;

public class Profile
{
    private readonly ILogger _logger;
    private readonly string _message;

    private readonly Stopwatch _stopwatch;

    public Profile(ILogger logger, string message)
    {
        _logger = logger;
        _message = message;

        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
        _logger.LogTrace(_message, new object[] { _stopwatch.ElapsedMilliseconds });
    }

    public void Stop(object[] args)
    {
        _stopwatch.Stop();
        _logger.LogTrace(_message, new object[] { _stopwatch.ElapsedMilliseconds }.Concat(args).ToArray());
    }
}