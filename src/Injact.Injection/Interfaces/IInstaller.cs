namespace Injact.Injection;

public interface IInstaller
{
    public DiContainer Container { get; }

    public void InstallBindings();
}