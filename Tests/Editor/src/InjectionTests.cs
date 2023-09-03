using Injact.Tests.Shared;
using NUnit.Framework;

namespace Injact.Tests.Editor
{
    public class InjectionTests
    {
        [Test]
        public void Inject_ConstructorInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestInterface_1, TestClass_1>();
            container.Bind<TestClass_2>();
            container.Bind<TestClass_ConstructorInjection>();
            container.ProcessPendingBindings();

            var resolved = container.Resolve<TestClass_ConstructorInjection>(null);

            Assert.IsNotNull(resolved.TestClass);
            Assert.IsNotNull(resolved.TestInterface);
        }

        [Test]
        public void Inject_PublicReadonlyFieldInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_PublicReadonly_FieldInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_ProtectedReadonlyFieldInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_ProtectedReadonly_FieldInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_PrivateReadonlyFieldInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_PrivateReadonly_FieldInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_PropertyInjection_Succeeds()
        {
            //Note: Property injection works by setting the value of the backing field
            //If it works on one property it will work on all of them regardless of access modifier or if there is a setter

            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_Public_PropertyInjection_NoSetter();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_PropertyInjection_InheritedProperty_Succeeds()
        {
            //Note: Property injection works by setting the value of the backing field
            //If it works on one property it will work on all of them regardless of access modifier or if there is a setter

            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_Public_PropertyInjection_NoSetter_Inherited();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_PublicMethodInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_Public_MethodInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_ProtectedMethodInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_Protected_MethodInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }

        [Test]
        public void Inject_PrivateMethodInjection_Succeeds()
        {
            var container = new DiContainer();

            container.Bind<TestClass_2>();
            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var instance = new TestClass_Private_MethodInjection();
            var injector = container.Resolve<Injector>(null);

            injector.InjectInto(instance);

            Assert.IsNotNull(instance.TestClass);
            Assert.IsNotNull(instance.TestInterface);
        }
    }
}