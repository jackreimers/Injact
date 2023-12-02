namespace Injact.Godot;

public abstract partial class NodeInstaller : Node, IInstaller
{
    [Inject] public DiContainer Container { get; } = null!;

    public abstract void InstallBindings();
}