namespace Injact;

/// <summary>
/// Wrapper class that provides access to the physical properties of a scene object.
/// </summary>
public interface IPhysicalProvider
{
    public Vector3 Position { get; }
    
    public void Translate(Vector3 translation);
    
    public void Translate(float x, float y, float z);
    
    public void TranslateLocal(Vector3 translation);
    
    public void TranslateLocal(float x, float y, float z);
    
    public void Rotate(Vector3 rotation);
    
    public void Rotate(float x, float y, float z);
}