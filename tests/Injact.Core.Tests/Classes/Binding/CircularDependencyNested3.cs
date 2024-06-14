namespace Injact.Tests.Classes;

public class CircularDependencyNested3
{
    private readonly CircularDependencyNested1 _testClass;

    public CircularDependencyNested3(CircularDependencyNested1 testClass)
    {
        _testClass = testClass;
    }
}
