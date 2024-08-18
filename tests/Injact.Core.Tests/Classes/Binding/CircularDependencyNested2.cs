namespace Injact.Tests.Classes;

public class CircularDependencyNested2
{
    private readonly CircularDependencyNested3 _testClass;

    public CircularDependencyNested2(CircularDependencyNested3 testClass)
    {
        _testClass = testClass;
    }
}
