using System.Collections.Generic;
using System.Linq;
using Godot;
using Injact.Internal;

namespace Injact;

public partial class Context : Node
{
    [Export] private bool injectIntoNodes = true;
    [Export] private bool searchForInstallers;
    [Export] private NodeInstaller[] installers;

    [Export] private bool loggingEnabled;
    [Export] private bool profilingEnabled;

    private DiContainer _container;
    private Injector _injector;

    public override void _EnterTree()
    {
        var profile = GodotHelpers.ProfileIf(profilingEnabled, "Initialised context in {0}ms.");

        _container = new DiContainer(loggingEnabled, profilingEnabled);
        _injector = _container.Resolve<Injector>(this);

        List<Node> nodes = null!;

        if (searchForInstallers)
        {
            var installerProfile = GodotHelpers.ProfileIf(profilingEnabled, "Found {1} installers in {0}ms.");

            GodotHelpers.WarnIf(installers.Any(), "Search for installers is enabled, user set installers will be ignored.");
            nodes = GodotHelpers.GetAllChildNodes(GetTree().Root);

            installers = nodes
                .Where(s => s is NodeInstaller)
                .Cast<NodeInstaller>()
                .ToArray();

            GodotHelpers.WarnIf(!installers.Any(), "Could not find any node installers in scene.");

            installerProfile?.Invoke(new object[] { installers.Length });
        }

        foreach (var installer in installers)
        {
            _injector.InjectInto(installer);
            installer.InstallBindings();
        }

        _container.ProcessPendingBindings();

        if (injectIntoNodes)
            ResolveAllInScene(nodes);

        profile?.Invoke(null);
        base._EnterTree();
    }

    private void ResolveAllInScene(List<Node> nodes)
    {
        var profile = GodotHelpers.ProfileIf(profilingEnabled, "Found {1} nodes in {0}ms.");
        nodes ??= GodotHelpers.GetAllChildNodes(GetTree().Root);

        foreach (var node in nodes)
            _injector.InjectInto(node);

        profile?.Invoke(new object[] { nodes.Count });
    }
}