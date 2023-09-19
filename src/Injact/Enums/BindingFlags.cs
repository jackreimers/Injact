using System;

namespace Injact;

[Flags]
public enum BindingFlags
{
    None = 0,
    Singleton = 1 << 0,
    Immediate = 1 << 1,
    Factory = 1 << 2,
}