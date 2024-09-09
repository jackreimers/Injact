﻿namespace Injact.Godot;

public abstract partial class NodeInstaller : Node, IInstaller
{
    [Inject] protected readonly DiContainer _container = null!;

    public abstract void InstallBindings();
}