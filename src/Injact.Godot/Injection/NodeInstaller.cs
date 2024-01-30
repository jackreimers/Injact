namespace Injact.Godot;

public abstract partial class NodeInstaller : Node, IInstaller
{
    [Inject] public DiContainer _container { get; } = null!;

    public abstract void InstallBindings();
}