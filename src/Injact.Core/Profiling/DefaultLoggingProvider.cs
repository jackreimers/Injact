namespace Injact;

public class DefaultLoggingProvider : ILoggingProvider
{
    public ILogger GetLogger<T>()
    {
        return new DefaultLogger<T>();
    }

    public Type GetLoggerType()
    {
        return typeof(DefaultLogger<>);
    }
}