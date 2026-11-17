using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class CircleTests
{
    [Fact]
    public void Constructor_SetsRadius()
    {
        var circle = new Circle(3);

        Assert.Equal(3, circle.Radius);
    }

    [Fact]
    public void Constructor_AllowsZeroRadius()
    {
        var circle = new Circle(0);

        Assert.Equal(0, circle.Radius);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Constructor_RejectsInvalidRadius(double radius)
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new Circle(radius));
    }

    [Fact]
    public void Area_ReturnsCorrectValue()
    {
        var circle = new Circle(2);

        Assert.Equal(4 * Math.PI, circle.Area(), precision: 12);
    }

    [Fact]
    public void Circumference_ReturnsCorrectValue()
    {
        var circle = new Circle(2);

        Assert.Equal(4 * Math.PI, circle.Circumference(), precision: 12);
    }

    [Fact]
    public void ZeroRadius_ReturnsZeroAreaAndCircumference()
    {
        var circle = new Circle(0);

        Assert.Equal(0, circle.Area());
        Assert.Equal(0, circle.Circumference());
    }
}