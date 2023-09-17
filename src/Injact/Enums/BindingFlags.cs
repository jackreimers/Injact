using System;

namespace Injact;

[Flags]
public enum BindingFlags
{
    Singleton = 1 << 0,
    Immediate = 1 << 1
}