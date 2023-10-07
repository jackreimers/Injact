using System.Linq;
using Godot;
using Injact.Godot.Profiling;
using Injact.Godot.Utility;
using Injact.Injection;
using Injact.Profiling;

namespace Injact.Godot.Injection;

public class Context : Node
{
    [ExportCategory("Initialisation")]
    [Export] private bool searchForNodes = true;
    [Export] private bool searchForInstallers = true;

    [ExportCategory("References")]
    [Export] private Node[] injectTargets;
    [Export] private NodeInstaller[] installers;
    
    private DiContainer _container;
    private Injector _injector;

    private ILogger _logger;
    private IProfiler _profiler;
    
    public override void _EnterTree()
    {
        _logger = new Logger<Context>();
        _profiler = new Profiler(_logger);
        
        _container = new DiContainer(_logger, _profiler);
        _injector = _container.Resolve<Injector>(this);
        
        _injector.InjectInto(this);
        
        TrySearchForInstallers();

        foreach (var installer in installers)
        {
            _injector.InjectInto(installer);
            installer.InstallBindings();
        }

        _container.ProcessPendingBindings();
        TryResolveAllInScene();
        
        foreach (var target in injectTargets)
            _injector.InjectInto(target);
    }

    private void TrySearchForInstallers()
    {
        if (!searchForInstallers)
            return;
        
        _logger.LogWarning("Search for nodes is enabled, user set nodes will be ignored.", injectTargets.Any());
        _logger.LogWarning("Search for installers is enabled, user set installers will be ignored.", installers.Any());
            
        installers = injectTargets
            .Where(s => s is NodeInstaller)
            .Cast<NodeInstaller>()
            .ToArray();

        _logger.LogWarning("Could not find any node installers in scene.", !installers.Any());
    }
    
    private void TryResolveAllInScene()
    {
        if (!searchForNodes)
            return;
        
        injectTargets ??= GodotHelpers.GetAllChildNodes(GetTree().Root);
    }
}