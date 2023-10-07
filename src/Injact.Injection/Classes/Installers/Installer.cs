namespace Injact.Injection;

public abstract class Installer : IInstaller
{
    [Inject] public DiContainer Container { get; } = null!;

    public abstract void InstallBindings();
}