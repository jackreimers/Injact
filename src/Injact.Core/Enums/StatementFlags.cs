using System;

namespace Injact;

[Flags]
public enum StatementFlags
{
    None = 0,
    Singleton = 1 << 0,
    Immediate = 1 << 1,
    Factory = 1 << 2,
}