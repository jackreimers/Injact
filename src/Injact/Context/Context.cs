using System.Linq;
using Godot;
using Injact.Internal;
using Injact.Profiling;

namespace Injact;

public partial class Context : Node
{
    [ExportCategory("Initialisation")]
    [Export] private bool searchForNodes = true;
    [Export] private bool searchForInstallers;
    
    [ExportCategory("References")]
    [Export] private Node[] nodes;
    [Export] private NodeInstaller[] installers;
    
    //TODO: Investigate better way to set flag options for export
    [ExportCategory("Logging")]
    [Export(PropertyHint.Flags, "Information")] private int loggingLevels = 0;
    [Export(PropertyHint.Flags, "Startup,Resolution")] private int profilingLevels = 0;

    [Inject] private readonly ILogger _logger = null!;
    [Inject] private readonly IProfiler _profiler = null!;

    private DiContainer _container;
    private Injector _injector;
    
    private LoggingFlags loggingFlags;
    private ProfilingFlags profilingFlags;

    public override void _EnterTree()
    {
        loggingFlags = (LoggingFlags)loggingLevels;
        profilingFlags = (ProfilingFlags)profilingLevels;

        _container = new DiContainer(loggingFlags, profilingFlags);
        _injector = _container.Resolve<Injector>(this);   
        
        _injector.InjectInto(this);
        
        var profile = _profiler.Start(ProfilingFlags.Startup, "Initialised dependency injection in {0}ms.");
        
        if (searchForInstallers)
        {
            var installerProfile = _profiler.Start(ProfilingFlags.Startup, "Found {1} installers in {0}ms.");

            _logger.LogWarning("Search for nodes is enabled, user set nodes will be ignored.", nodes.Any());
            _logger.LogWarning("Search for installers is enabled, user set installers will be ignored.", installers.Any());
            
            nodes = GodotHelpers.GetAllChildNodes(GetTree().Root);

            installers = nodes
                .Where(s => s is NodeInstaller)
                .Cast<NodeInstaller>()
                .ToArray();

            _logger.LogWarning("Could not find any node installers in scene.", !installers.Any());

            installerProfile?.Stop(new object[] { installers.Length });
        }

        foreach (var installer in installers)
        {
            _injector.InjectInto(installer);
            installer.InstallBindings();
        }

        _container.ProcessPendingBindings();

        if (searchForNodes)
            ResolveAllInScene();

        profile?.Stop();
        base._EnterTree();
    }

    private void ResolveAllInScene()
    {
        var profile = _profiler.Start(ProfilingFlags.Startup, "Found {1} nodes in {0}ms.");
        nodes ??= GodotHelpers.GetAllChildNodes(GetTree().Root);

        foreach (var node in nodes)
            _injector.InjectInto(node);

        profile?.Stop(new object[] { nodes.Length });
    }
}