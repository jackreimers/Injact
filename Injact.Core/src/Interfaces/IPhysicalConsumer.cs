namespace Injact;

public interface IPhysicalConsumer : ILifecycleConsumer
{
    public IPhysicalProvider PhysicalProvider { get; set; }
}