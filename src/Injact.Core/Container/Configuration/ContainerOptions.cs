namespace Injact.Container.Configuration;

public class ContainerOptions
{
    public LoggingLevel LoggingLevel { get; set; }

    public bool LogTracing { get; set; }
    public bool UseAutoFactories { get; set; } = true;
    public bool InjectIntoDefaultProperties { get; set; }

    public ILoggingProvider LoggingProvider { get; init; } = null!;
}