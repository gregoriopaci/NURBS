
using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class Point3DTests
{
    [Fact]
    public void Constructor_InitializesCoordinates()
    {
        var point = new Point3D(1, 2, 3);

        Assert.Equal(1, point.X);
        Assert.Equal(2, point.Y);
        Assert.Equal(3, point.Z);
    }

    [Fact]
    public void Properties_CanBeModified()
    {
        var point = new Point3D(1, 2, 3);

        point.X = 4;
        point.Y = 5;
        point.Z = 6;

        Assert.Equal(4, point.X);
        Assert.Equal(5, point.Y);
        Assert.Equal(6, point.Z);
    }

    [Fact]
    public void DistanceTo_ReturnsCorrectEuclideanDistance()
    {
        var p1 = new Point3D(0, 0, 0);
        var p2 = new Point3D(2, 3, 6);

        Assert.Equal(7, p1.DistanceTo(p2), 12);
    }

    [Fact]
    public void Distance_ReturnsSameResultAsDistanceTo()
    {
        var p1 = new Point3D(1, 2, 3);
        var p2 = new Point3D(4, 6, 3);

        Assert.Equal(p1.DistanceTo(p2), Point3D.Distance(p1, p2), 12);
    }

    [Fact]
    public void Subtract_ReturnsDisplacementVector()
    {
        var p1 = new Point3D(4, 6, 8);
        var p2 = new Point3D(1, 2, 3);

        var result = p1.Subtract(p2);

        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
        Assert.Equal(5, result.Z);
    }

    [Fact]
    public void PointDifference_ReturnsDisplacementVector()
    {
        var p1 = new Point3D(4, 6, 8);
        var p2 = new Point3D(1, 2, 3);

        Vector3D result = p1 - p2;

        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
        Assert.Equal(5, result.Z);
    }

    [Fact]
    public void Translate_ReturnsTranslatedPoint()
    {
        var point = new Point3D(1, 2, 3);
        var vector = new Vector3D(2, -1, 4);

        var result = point.Translate(vector);

        Assert.Equal(3, result.X);
        Assert.Equal(1, result.Y);
        Assert.Equal(7, result.Z);
    }

    [Fact]
    public void PointPlusVector_ReturnsTranslatedPoint()
    {
        var point = new Point3D(1, 2, 3);
        var vector = new Vector3D(2, -1, 4);

        Point3D result = point + vector;

        Assert.Equal(3, result.X);
        Assert.Equal(1, result.Y);
        Assert.Equal(7, result.Z);
    }

    [Fact]
    public void Lerp_AtZero_ReturnsFirstPoint()
    {
        var p1 = new Point3D(1, 2, 3);
        var p2 = new Point3D(5, 6, 7);

        var result = Point3D.Lerp(p1, p2, 0);

        Assert.Equal(p1.X, result.X);
        Assert.Equal(p1.Y, result.Y);
        Assert.Equal(p1.Z, result.Z);
    }

    [Fact]
    public void Lerp_AtOne_ReturnsSecondPoint()
    {
        var p1 = new Point3D(1, 2, 3);
        var p2 = new Point3D(5, 6, 7);

        var result = Point3D.Lerp(p1, p2, 1);

        Assert.Equal(p2.X, result.X);
        Assert.Equal(p2.Y, result.Y);
        Assert.Equal(p2.Z, result.Z);
    }

    [Fact]
    public void Lerp_AtHalfReturnsMidpoint()
    {
        var p1 = new Point3D(0, 0, 0);
        var p2 = new Point3D(4, 6, 8);

        var result = Point3D.Lerp(p1, p2, 0.5);

        Assert.Equal(2, result.X);
        Assert.Equal(3, result.Y);
        Assert.Equal(4, result.Z);
    }
}
