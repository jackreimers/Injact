using Injact.Tests.Shared;
using NUnit.Framework;

namespace Injact.Tests.Editor
{
    public class BindingTests
    {
        [Test]
        public void Bind_DefaultBindings_AvailableAfterInitialisation()
        {
            var container = new DiContainer();

            var resolvedContainer = container.Resolve<DiContainer>(null);
            var resolvedInjector = container.Resolve<Injector>(null);

            Assert.IsNotNull(resolvedContainer);
            Assert.IsNotNull(resolvedInjector);
        }

        [Test]
        public void Bind_InterfaceToContainer_ReturnsInstanceForInterface()
        {
            var container = new DiContainer();

            container.Bind<TestInterface_1, TestClass_1>();
            container.ProcessPendingBindings();

            var resolved = container.Resolve<TestInterface_1>(null);

            Assert.IsNotNull(resolved);
        }

        [Test]
        public void Bind_ConcreteToContainer_ReturnsInstanceForConcrete()
        {
            var container = new DiContainer();

            container.Bind<TestClass_1>();
            container.ProcessPendingBindings();

            var resolved = container.Resolve<TestClass_1>(null);

            Assert.IsNotNull(resolved);
        }

        [Test]
        public void Bind_SingletonToContainer_ReturnsSameInstance()
        {
            var container = new DiContainer();

            container.Bind<TestClass_1>().AsSingleton();
            container.ProcessPendingBindings();

            var resolved1 = container.Resolve<TestClass_1>(null);
            var resolved2 = container.Resolve<TestClass_1>(null);

            Assert.AreSame(resolved1, resolved2);
        }

        [Test]
        public void Bind_SingletonToContainer_ReturnsExistingInstance()
        {
            var instance = new TestClass_1();
            var container = new DiContainer();

            container.Bind<TestClass_1>().FromInstance(instance).AsSingleton();
            container.ProcessPendingBindings();

            var resolved = container.Resolve<TestClass_1>(null);

            Assert.AreSame(instance, resolved);
        }

        [Test]
        public void Bind_NonSingletonToContainer_ReturnsDifferentInstances()
        {
            var container = new DiContainer();

            container.Bind<TestClass_1>().AsTransient();
            container.ProcessPendingBindings();

            var resolved1 = container.Resolve<TestClass_1>(null);
            var resolved2 = container.Resolve<TestClass_1>(null);

            Assert.AreNotSame(resolved1, resolved2);
        }
    }
}