using System;

namespace Injact;

public interface ILifecycleProvider
{
    public event Action<double> OnUpdate;
}