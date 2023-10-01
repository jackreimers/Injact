namespace Injact;

public enum ProfilingFlags
{
    None = 0,
    Startup = 1 << 0,
    Resolution = 1 << 1,
    External = 1 << 2,
}