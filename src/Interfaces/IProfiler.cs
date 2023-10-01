namespace Injact;

public interface IProfiler
{
    public Profile Start(ProfilingFlags profilingLevel, string message);
}