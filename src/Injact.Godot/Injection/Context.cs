﻿using Time = Injact.Spatial.Time;

namespace Injact.Godot;

public partial class Context : Node
{
    [Inject] private readonly ILogger _logger = null!;

    [ExportCategory("Initialisation")] [Export]
    private bool searchForNodes = true;
    [Export] private bool searchForInstallers = true;

    [ExportCategory("References")] [Export]
    private Node[] injectTargets = Array.Empty<Node>();
    [Export] private NodeInstaller[] installers = Array.Empty<NodeInstaller>();

    [ExportCategory("Logging")] [Export] private LoggingLevel loggingLevel = LoggingLevel.Information;
    [Export] private bool logDebugging = true;
    [Export] private bool logTracing = true;

    private DiContainer _container = null!;
    private ContainerOptions? _containerOptions;
    private Injector _injector = null!;

    private Node[]? nodeBuffer;
    private IInstaller[] nativeInstallers = Array.Empty<IInstaller>();

    public override void _EnterTree()
    {
        //Note that logging settings will not be updated at runtime
        _container = new DiContainer(_containerOptions ?? new ContainerOptions
        {
            LoggingLevel = loggingLevel,
            LogTracing = logTracing,
            LoggingProvider = new LoggingProvider()
        });

        _injector = _container.Resolve<Injector>(this);
        _injector.InjectInto(this);

        TrySearchForInstallers();
        TryResolveAllInScene();

        foreach (var installer in nativeInstallers.Concat(installers))
        {
            _logger.LogInformation($"Installing bindings for {installer.GetType().Name}.");
            _injector.InjectInto(installer);

            installer.InstallBindings();
        }

        foreach (var target in injectTargets)
        {
            _injector.InjectInto(target);
        }
    }

    public override void _Process(double delta)
    {
        Time.Delta = (float)delta;
        Time.TriggerUpdate();
        Time.TriggerLateUpdate();

        base._Process(delta);
    }

    protected void SetContainerOptions(ContainerOptions options)
    {
        _containerOptions = options;
    }

    protected void AddInstallers(params IInstaller[] value)
    {
        nativeInstallers = value;
    }

    private void TrySearchForInstallers()
    {
        if (!searchForInstallers)
        {
            return;
        }

        _logger.LogWarning("Search for installers is enabled, user set installers will be ignored.", installers.Any());

        nodeBuffer ??= GodotHelper.GetAllChildNodes(GetTree().Root);
        installers = nodeBuffer
            .Where(s => s is NodeInstaller)
            .Cast<NodeInstaller>()
            .ToArray();

        _logger.LogWarning("Could not find any node installers in scene.", !installers.Any());
    }

    private void TryResolveAllInScene()
    {
        if (!searchForNodes)
        {
            return;
        }

        _logger.LogWarning("Search for nodes is enabled, user set nodes will be ignored.", injectTargets.Any());

        nodeBuffer ??= GodotHelper.GetAllChildNodes(GetTree().Root);
        injectTargets = nodeBuffer;
    }
}