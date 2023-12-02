using Injact.Tests.Classes;

namespace Injact.Tests.Core;

public class InjectionTests
{
    [Fact]
    public void Inject_ConstructorInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestInterface1, TestClass1>();
        container.Bind<TestClass2>();
        container.Bind<TestClass_ConstructorInjection>();
        container.ProcessPendingBindings();

        var resolved = container.Resolve<TestClass_ConstructorInjection>(typeof(TestConsumer1));

        Assert.NotNull(resolved);
        Assert.NotNull(resolved.TestClass);
        Assert.NotNull(resolved.TestInterface);
    }

    [Fact]
    public void Inject_PublicReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_PublicReadonly_FieldInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_ProtectedReadonly_FieldInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));

        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateReadonlyFieldInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_PrivateReadonly_FieldInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));
        
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

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_Public_PropertyInjection_NoSetter();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));

        Assert.NotNull(injector);
        
        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PublicMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_Public_MethodInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));
        
        Assert.NotNull(injector);
        
        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_ProtectedMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_Protected_MethodInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));
        
        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }

    [Fact]
    public void Inject_PrivateMethodInjection_Succeeds()
    {
        var container = new DiContainer();

        container.Bind<TestClass2>();
        container.Bind<TestInterface1, TestClass1>();
        container.ProcessPendingBindings();

        var instance = new TestClass_Private_MethodInjection();
        var injector = container.Resolve<Injector>(typeof(TestConsumer1));
        
        Assert.NotNull(injector);

        injector.InjectInto(instance);

        Assert.NotNull(instance.TestClass);
        Assert.NotNull(instance.TestInterface);
    }
}