using System.Numerics;

namespace Injact.Engine;

public interface IPhysicalProvider : ILifecycleProvider
{
    public void Translate(Vector3 translation);
    
    public void TranslateLocal(Vector3 translation);
    
    public void Rotate(Vector3 rotation);
    
    public void Rotate(float x, float y, float z);
}