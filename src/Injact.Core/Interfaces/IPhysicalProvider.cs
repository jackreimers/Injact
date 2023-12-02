namespace Injact;

public interface IPhysicalProvider
{
    public Vector3 Position { get; }
    
    public void Translate(Vector3 translation);
    
    public void TranslateLocal(Vector3 translation);
    
    public void Rotate(Vector3 rotation);
    
    public void Rotate(float x, float y, float z);
}