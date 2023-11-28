namespace Injact;

public class ContainerOptions
{
    public ILoggingProvider LoggingProvider { get; set; }
    
    public bool LogDebugging { get; set; }
    public bool LogTracing { get; set; }
    
    public bool AllowOnDemandFactories { get; set; } 
}