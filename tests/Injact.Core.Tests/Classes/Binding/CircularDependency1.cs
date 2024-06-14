namespace Injact.Tests.Classes;

public class CircularDependency1
{
    private readonly CircularDependency2 _testClass;

    public CircularDependency1(CircularDependency2 testClass)
    {
        _testClass = testClass;
    }
}
