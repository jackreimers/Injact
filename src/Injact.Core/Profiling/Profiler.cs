namespace Injact;

public class Profiler : IProfiler
{
    private readonly ILogger _logger;

    public Profiler(ILogger logger)
    {
        _logger = logger;
    }

    public Profile Start(string message)
    {
        return new Profile(_logger, message);
    }
}