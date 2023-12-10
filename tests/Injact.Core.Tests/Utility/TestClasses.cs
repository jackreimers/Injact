namespace Injact.Tests.Classes;

public class TestConsumer1 { }

public class TestConsumer2 { }

public class TestClass1 : TestInterface1 { }

public class TestClass2 : TestInterface1 { }

public class TestClass_ConstructorInjection
{
    public TestClass_ConstructorInjection(TestInterface1 testInterface, TestClass2 testClass)
    {
        TestInterface = testInterface;
        TestClass = testClass;
    }

    public TestInterface1 TestInterface { get; }
    public TestClass2 TestClass { get; }
}

public class TestClass_PublicReadonly_FieldInjection
{
    [Inject] public readonly TestClass2 TestClass = null!;
    [Inject] public readonly TestInterface1 TestInterface = null!;
}

public class TestClass_ProtectedReadonly_FieldInjection
{
    [Inject] protected readonly TestClass2 testClass = null!;
    [Inject] protected readonly TestInterface1 testInterface = null!;

    public TestClass2 TestClass => testClass;
    public TestInterface1 TestInterface => testInterface;
}

public class TestClass_PrivateReadonly_FieldInjection
{
    [field: Inject] public TestClass2 TestClass { get; } = null!;
    [field: Inject] public TestInterface1 TestInterface { get; } = null!;
}

public class TestClass_Public_PropertyInjection_NoSetter
{
    [Inject] public TestClass2 TestClass { get; } = null!;
    [Inject] public TestInterface1 TestInterface { get; } = null!;
}

public class TestClass_Public_MethodInjection
{
    public TestClass2 TestClass { get; private set; } = null!;
    public TestInterface1 TestInterface { get; private set; } = null!;

    [Inject]
    public void Inject(TestClass2 testClass, TestInterface1 testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}

public class TestClass_Protected_MethodInjection
{
    public TestClass2 TestClass { get; private set; } = null!;
    public TestInterface1 TestInterface { get; private set; } = null!;

    [Inject]
    protected void Inject(TestClass2 testClass, TestInterface1 testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}

public class TestClass_Private_MethodInjection
{
    public TestClass2 TestClass { get; private set; } = null!;
    public TestInterface1 TestInterface { get; private set; } = null!;

    [Inject]
    private void Inject(TestClass2 testClass, TestInterface1 testInterface1)
    {
        TestClass = testClass;
        TestInterface = testInterface1;
    }
}