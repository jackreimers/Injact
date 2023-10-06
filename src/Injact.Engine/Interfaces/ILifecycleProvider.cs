using System;

namespace Injact.Engine;

public interface ILifecycleProvider
{
    public event Action<double> Update;
}