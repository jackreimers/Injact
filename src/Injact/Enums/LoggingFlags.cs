using System;

namespace Injact;

[Flags]
public enum LoggingFlags
{
    None = 0,
    Verbose = 1 << 0,
    Information = 1 << 1,
    Warning = 1 << 2,
}