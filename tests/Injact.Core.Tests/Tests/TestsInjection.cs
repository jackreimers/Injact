﻿namespace Injact.Tests;

public class TestsInjection
{
    [Fact]
    public void Inject_ConstructorInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<ConstructorInjection>()
            .Finalise();

        container.ProcessPendingBindings();

        var resolved = container.Resolve<ConstructorInjection>(typeof(Consumer));

        Assert.NotNull(resolved);
        Assert.NotNull(resolved.TestClass);
        Assert.NotNull(resolved.TestInterface);
    }

    [Fact]
    public void Inject_PublicReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionFieldReadonlyPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionFieldReadonlyProtected();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionFieldReadonlyPrivate();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PropertyInjection_Succeeds()
    {
        //Note: Property injection works by setting the value of the backing field
        //If it works on one property it will work on all of them regardless of access modifier or if there is a setter

        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionPropertyPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PublicMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionMethodPublic();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionMethodProtected();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container
            .Bind<Class2>()
            .Finalise();

        container
            .Bind<Interface1, Class1>()
            .Finalise();

        container.ProcessPendingBindings();

        var instance = new InjectionMethodPrivate();
        var injector = container.Resolve<Injector>(typeof(Consumer));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_IllegalInjection_ThrowsException()
    {
        var container = new DiContainer();

        container
            .Bind<Interface1, Class1>()
            .WhenInjectedInto<Class2>()
            .Finalise();

        container
            .Bind<ConstructorInjection>()
            .Finalise();

        container.ProcessPendingBindings();

        Assert.Throws<DependencyException>(() =>
            container.Resolve<ConstructorInjection>(typeof(Consumer)));
    }
}
