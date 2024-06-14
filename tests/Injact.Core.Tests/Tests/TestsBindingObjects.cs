namespace Injact.Tests;

public class TestsBindingObjects
{
    [Fact]
    public void Bind_DefaultBindings_Resolves()
    {
        var container = new DiContainer();

        var resolvedContainer = container.Resolve<DiContainer>(typeof(Consumer));
        var resolvedInjector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(resolvedContainer);
        Assert.NotNull(resolvedInjector);
    }

    [Fact]
    public void Bind_InterfaceToContainer_ReturnsInstanceForInterface()
    {
        var container = new DiContainer();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved = container.Resolve<Interface1>(typeof(Consumer));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_ConcreteToContainer_ReturnsInstanceForConcrete()
    {
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved = container.Resolve<Class1>(typeof(Consumer));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsSameInstance()
    {
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .AsSingleton()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved1 = container.Resolve<Class1>(typeof(Consumer));
        var resolved2 = container.Resolve<Class1>(typeof(Consumer));

        Assert.Same(resolved1, resolved2);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsExistingInstance()
    {
        var instance = new Class1();
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .FromInstance(instance)
            .AsSingleton()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved = container.Resolve<Class1>(typeof(Consumer));

        Assert.Same(instance, resolved);
    }

    [Fact]
    public void Bind_NonSingletonToContainer_ReturnsDifferentInstances()
    {
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .AsTransient()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved1 = container.Resolve<Class1>(typeof(Consumer));
        var resolved2 = container.Resolve<Class1>(typeof(Consumer));

        Assert.NotSame(resolved1, resolved2);
    }

    [Fact]
    public void Bind_ExistingBinding_ThrowsException()
    {
        var container = new DiContainer();

        container
            .Bind<Class1>()
            .Finalise();

        container
            .Bind<Class1>()
            .Finalise();

        Assert.Throws<DependencyException>(() => container.ProcessPendingBindings());
    }

    [Fact]
    public void Bind_CircularDependency_ThrowsException()
    {
        var container = new DiContainer();

        container
            .Bind<CircularDependency1>()
            .Finalise();

        container
            .Bind<CircularDependency2>()
            .Finalise();

        container.ProcessPendingBindings();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<CircularDependency1>(typeof(Consumer)));
    }

    [Fact]
    public void Bind_CircularDependencyNested_ThrowsException()
    {
        var container = new DiContainer();

        container
            .Bind<CircularDependencyNested1>()
            .Finalise();

        container
            .Bind<CircularDependencyNested2>()
            .Finalise();

        container
            .Bind<CircularDependencyNested3>()
            .Finalise();

        container.ProcessPendingBindings();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<CircularDependencyNested1>(typeof(Consumer)));
    }
}
