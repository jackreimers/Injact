using System.Numerics;

namespace Injact.Engine;

public interface IPhysicalProvider
{
    public Vector3 Position { get; }
    
    public void Translate(System.Numerics.Vector3 translation);
    
    public void TranslateLocal(System.Numerics.Vector3 translation);
    
    public void Rotate(System.Numerics.Vector3 rotation);
    
    public void Rotate(float x, float y, float z);
}