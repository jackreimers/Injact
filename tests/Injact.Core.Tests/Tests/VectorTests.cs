namespace Injact.Tests;

public class VectorTests
{
    private static readonly Vector3 vector1 = new Vector3(1, 2, 4);
    private static readonly Vector3 vector2 = new Vector3(2, 4, 6);
    private static readonly Vector3 vector3 = new Vector3(0, 0, 0);
    private static readonly Vector3 vector4 = new Vector3(5, 0, 5);
    private static readonly Vector3 vector5 = new Vector3(5, 1, 3);
    private static readonly Vector3 vector6 = new Vector3(-5, -1, -10);

    [Fact]
    public void Vector_Pivot_ReturnsCorrectValue()
    {
        var vec1 = Vector3.Zero;
        var vec2 = new Vector3(0, 0, 3);

        var pivot = Vector3.Pivot(vec2, vec1, Vector3.Up, 90);

        Assert.True(new Vector3(3f, 0f, 0f) == pivot);
    }

    //TODO: Use theory to test all operators
    [Fact]
    public void Vector_Dot_ReturnsCorrectValue()
    {
        Assert.Equal(
            34,
            Vector3.Dot(vector1, vector2));

        Assert.Equal(
            0,
            Vector3.Dot(vector3, vector4));

        Assert.Equal(
            -56,
            Vector3.Dot(vector5, vector6));
    }

    [Fact]
    public void Vector_Cross_ReturnsCorrectValue()
    {
        Assert.Equal(
            new Vector3(-4, 2, 0),
            Vector3.Cross(vector1, vector2));

        Assert.Equal(
            new Vector3(0, 0, 0),
            Vector3.Cross(vector3, vector4));

        Assert.Equal(
            new Vector3(-7, 35, 0),
            Vector3.Cross(vector5, vector6));
    }

    [Fact]
    public void Vector_Distance_ReturnsCorrectValue()
    {
        Assert.Equal(
            3,
            Vector3.Distance(vector1, vector2));

        Assert.Equal(
            7.071068f,
            Vector3.Distance(vector3, vector4));

        Assert.Equal(
            16.522712f,
            Vector3.Distance(vector5, vector6));
    }
}