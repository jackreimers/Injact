using Injact.Tests.Classes;

namespace Injact.Tests;

public class BindingTests
{
    [Fact]
    public void Bind_DefaultBindings_AvailableAfterInitialisation()
    {
        var container = new DiContainer();

        var resolvedContainer = container.Resolve<DiContainer>(typeof(TestConsumer1));
        var resolvedInjector = container.Resolve<Injector>(typeof(TestConsumer1));

        Assert.NotNull(resolvedContainer);
        Assert.NotNull(resolvedInjector);
    }

    [Fact]
    public void Bind_InterfaceToContainer_ReturnsInstanceForInterface()
    {
        var container = new DiContainer();

        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var resolved = container.Resolve<TestInterface1>(typeof(TestConsumer1));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_ConcreteToContainer_ReturnsInstanceForConcrete()
    {
        var container = new DiContainer();

        container.Bind<TestClass1>();
        container.ProcessPendingBindings();

        var resolved = container.Resolve<TestClass1>(typeof(TestConsumer1));

        Assert.NotNull(resolved);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsSameInstance()
    {
        var container = new DiContainer();

        container.Bind<TestClass1>().AsSingleton();
        container.ProcessPendingBindings();

        var resolved1 = container.Resolve<TestClass1>(typeof(TestConsumer1));
        var resolved2 = container.Resolve<TestClass1>(typeof(TestConsumer1));

        Assert.Same(resolved1, resolved2);
    }

    [Fact]
    public void Bind_SingletonToContainer_ReturnsExistingInstance()
    {
        var instance = new TestClass1();
        var container = new DiContainer();

        container.Bind<TestClass1>().FromInstance(instance).AsSingleton();
        container.ProcessPendingBindings();

        var resolved = container.Resolve<TestClass1>(typeof(TestConsumer1));

        Assert.Same(instance, resolved);
    }

    [Fact]
    public void Bind_NonSingletonToContainer_ReturnsDifferentInstances()
    {
        var container = new DiContainer();

        container.Bind<TestClass1>().AsTransient();
        container.ProcessPendingBindings();

        var resolved1 = container.Resolve<TestClass1>(typeof(TestConsumer1));
        var resolved2 = container.Resolve<TestClass1>(typeof(TestConsumer1));

        Assert.NotSame(resolved1, resolved2);
    }
}