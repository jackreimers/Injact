namespace Injact;

public interface IPhysicalObject : ILifecycleObject
{
    public IPhysicalProvider PhysicalProvider { get; set; }
}