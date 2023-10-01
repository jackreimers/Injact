namespace Injact;

public class Profiler : IProfiler
{
    private readonly ILogger _logger;
    private readonly ProfilingFlags _profilingFlags;
    
    public Profiler(ILogger logger, ProfilingFlags profilingFlags)
    {
        _logger = logger;
        _profilingFlags = profilingFlags;
    }

    public Profile Start(ProfilingFlags profilingLevel, string message)
    {
        return _profilingFlags.HasFlag(profilingLevel)
            ? new Profile(_logger, message)
            : null;
    }
}