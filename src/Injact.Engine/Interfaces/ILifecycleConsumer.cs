namespace Injact.Engine;

public interface ILifecycleConsumer
{
    public void Awake() { }
    
    public void Start() { }
    
    public void Update(double delta) { }
}