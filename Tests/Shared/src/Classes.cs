// ReSharper disable MemberCanBePrivate.Global

namespace Injact.Tests.Shared
{
    public class TestClass_1 : TestInterface_1 { }

    public class TestClass_2 : TestInterface_1 { }

    public class TestClass_ConstructorInjection
    {
        public TestClass_ConstructorInjection(TestInterface_1 testInterface, TestClass_2 testClass)
        {
            TestInterface = testInterface;
            TestClass = testClass;
        }

        public TestInterface_1 TestInterface { get; }
        public TestClass_2 TestClass { get; }
    }

    public class TestClass_PublicReadonly_FieldInjection
    {
        [Inject] public readonly TestClass_2 TestClass = null!;
        [Inject] public readonly TestInterface_1 TestInterface = null!;
    }

    public class TestClass_ProtectedReadonly_FieldInjection
    {
        [Inject] protected readonly TestClass_2 testClass = null!;
        [Inject] protected readonly TestInterface_1 testInterface = null!;

        public TestClass_2 TestClass => testClass;
        public TestInterface_1 TestInterface => testInterface;
    }

    public class TestClass_PrivateReadonly_FieldInjection
    {
        [field: Inject] public TestClass_2 TestClass { get; } = null!;
        [field: Inject] public TestInterface_1 TestInterface { get; } = null!;
    }

    public class TestClass_Public_PropertyInjection_NoSetter
    {
        [Inject] public TestClass_2 TestClass { get; } = null!;
        [Inject] public TestInterface_1 TestInterface { get; } = null!;
    }

    public class TestClass_Public_MethodInjection
    {
        public TestClass_2 TestClass { get; private set; }
        public TestInterface_1 TestInterface { get; private set; }

        [Inject]
        public void Inject(TestClass_2 testClass, TestInterface_1 testInterface1)
        {
            TestClass = testClass;
            TestInterface = testInterface1;
        }
    }

    public class TestClass_Protected_MethodInjection
    {
        public TestClass_2 TestClass { get; private set; }
        public TestInterface_1 TestInterface { get; private set; }

        [Inject]
        protected void Inject(TestClass_2 testClass, TestInterface_1 testInterface1)
        {
            TestClass = testClass;
            TestInterface = testInterface1;
        }
    }

    public class TestClass_Private_MethodInjection
    {
        public TestClass_2 TestClass { get; private set; }
        public TestInterface_1 TestInterface { get; private set; }

        [Inject]
        private void Inject(TestClass_2 testClass, TestInterface_1 testInterface1)
        {
            TestClass = testClass;
            TestInterface = testInterface1;
        }
    }
}