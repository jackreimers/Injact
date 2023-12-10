namespace Injact;

public abstract class Installer : IInstaller
{
    protected Installer(DiContainer container)
    {
        Container = container;
    }
    
    public DiContainer Container { get; } 

    public abstract void InstallBindings();
}