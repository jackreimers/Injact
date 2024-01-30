namespace Injact;

public class ContainerOptions
{
    public bool LogDebugging { get; init; }
    public bool LogTracing { get; init; }
    public bool UseAutoFactories { get; init; }

    public ILoggingProvider LoggingProvider { get; init; } = null!;
}