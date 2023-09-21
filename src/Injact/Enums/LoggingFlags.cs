using System;

namespace Injact;

[Flags]
public enum LoggingFlags
{
    None = 0,
    Information = 1 << 0,
    Warning = 1 << 1,
}