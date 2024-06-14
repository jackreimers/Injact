namespace Injact.Tests.Classes;

public class CircularDependencyNested1
{
    private readonly CircularDependencyNested2 _testClass;

    public CircularDependencyNested1(CircularDependencyNested2 testClass)
    {
        _testClass = testClass;
    }
}
