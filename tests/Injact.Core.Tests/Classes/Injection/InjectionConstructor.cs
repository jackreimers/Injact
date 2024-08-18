namespace Injact.Tests.Classes;

public class ConstructorInjection
{
    public ConstructorInjection(Interface1 testInterface, Class2 testClass)
    {
        TestInterface = testInterface;
        TestClass = testClass;
    }

    public Interface1 TestInterface { get; }
    public Class2 TestClass { get; }
}
