namespace Injact.Engine;

public interface IPhysicalConsumer : ILifecycleConsumer
{
    public IPhysicalProvider PhysicalProvider { get; set; }
}