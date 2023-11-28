﻿using System.Linq;
using Godot;

namespace Injact.Godot;

public abstract partial class Context : Node
{
    [Inject] private readonly ILogger _logger = null!;

    [ExportCategory("Initialisation")] [Export]
    private bool searchForNodes = true;
    [Export] private bool searchForInstallers = true;

    [ExportCategory("References")] [Export]
    private Node[] injectTargets;
    [Export] private NodeInstaller[] installers;

    private DiContainer _container;
    private ContainerOptions _options;
    private Injector _injector;

    private Node[] nodeBuffer;
    private IInstaller[] nativeInstallers;

    public override void _EnterTree()
    {
        _container = new DiContainer(_options ?? new ContainerOptions { LoggingProvider = new LoggingProvider() });
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

        _container.ProcessPendingBindings();

        foreach (var target in injectTargets)
            _injector.InjectInto(target);
    }

    protected void SetContainerOptions(ContainerOptions options)
    {
        _options = options;
    }

    protected void AddInstallers(params IInstaller[] value)
    {
        nativeInstallers = value;
    }

    private void TrySearchForInstallers()
    {
        if (!searchForInstallers)
            return;

        _logger.LogWarning("Search for installers is enabled, user set installers will be ignored.", installers.Any());

        nodeBuffer ??= GodotHelpers.GetAllChildNodes(GetTree().Root);
        installers = nodeBuffer
            .Where(s => s is NodeInstaller)
            .Cast<NodeInstaller>()
            .ToArray();

        _logger.LogWarning("Could not find any node installers in scene.", !installers.Any());
    }

    private void TryResolveAllInScene()
    {
        if (!searchForNodes)
            return;

        _logger.LogWarning("Search for nodes is enabled, user set nodes will be ignored.", injectTargets.Any());

        nodeBuffer ??= GodotHelpers.GetAllChildNodes(GetTree().Root);
        injectTargets = nodeBuffer;
    }
}