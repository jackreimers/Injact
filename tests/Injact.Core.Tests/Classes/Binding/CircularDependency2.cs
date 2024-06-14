namespace Injact.Tests.Classes;

public class CircularDependency2
{
    private readonly CircularDependency1 _testClass;

    public CircularDependency2(CircularDependency1 testClass)
    {
        _testClass = testClass;
    }
}
