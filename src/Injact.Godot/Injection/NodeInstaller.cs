namespace Injact.Godot;

public abstract partial class NodeInstaller : Node, IInstaller
{
    [Inject] protected DiContainer _container = null!;

    public abstract void InstallBindings();
}
