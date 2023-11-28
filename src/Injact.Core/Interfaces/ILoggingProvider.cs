using System;

namespace Injact;

public interface ILoggingProvider
{
    public ILogger GetLogger<T>();

    public Type GetLoggerType();
}