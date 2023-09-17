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
        _container = new DiContainer(loggingEnabled, profilingEnabled);
        _injector = _container.Resolve<Injector>(this);

        List<Node> nodes = null!;

        if (searchForInstallers)
        {
            var profile = GodotHelpers.ProfileIf(profilingEnabled, "Found installers in {0}ms.");
                
            GodotHelpers.WarnIf(installers.Any(), "Search for installers is enabled, user set installers will be ignored.");
            nodes = GodotHelpers.GetAllChildNodes(GetTree().Root);
                
            installers = nodes
                .Where(s => s is NodeInstaller)
                .Cast<NodeInstaller>()
                .ToArray();
                
            GodotHelpers.PrintIf(loggingEnabled, $"Found {installers.Length} installers in scene.");
            GodotHelpers.WarnIf(!installers.Any(), "Could not find any node installers in scene.");
                
            profile?.Invoke();
        }
            
        foreach (var installer in installers)
        {
            _injector.InjectInto(installer);
            installer.InstallBindings();
        }

        _container.ProcessPendingBindings();

        if (injectIntoNodes)
            ResolveAllInScene(nodes);

        base._EnterTree();
    }

    private void ResolveAllInScene(List<Node> nodes)
    {
        var profile = GodotHelpers.ProfileIf(profilingEnabled, "Resolved all scene nodes in {0}ms.");
        nodes ??= GodotHelpers.GetAllChildNodes(GetTree().Root);
            
        GodotHelpers.PrintIf(loggingEnabled, $"Found {nodes.Count} nodes in scene.");

        foreach (var node in nodes)
            _injector.InjectInto(node);
            
        profile?.Invoke();
    }
}