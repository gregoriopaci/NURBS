using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class Vector3DTests
{
    [Fact]
    public void Constructor_InitializesCoordinates()
    {
        var vector = new Vector3D(1, 2, 3);

        Assert.Equal(1, vector.X);
        Assert.Equal(2, vector.Y);
        Assert.Equal(3, vector.Z);
    }

    [Fact]
    public void Norm_ReturnsCorrectLength()
    {
        var vector = new Vector3D(2, 3, 6);

        Assert.Equal(7, vector.Norm());
    }

    [Fact]
    public void DotProduct_ReturnsCorrectResult()
    {
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        Assert.Equal(32, v1 * v2);
        Assert.Equal(32, v1.Dot(v2));
    }

    [Fact]
    public void CrossProduct_ReturnsCorrectVector()
    {
        var v1 = new Vector3D(1, 0, 0);
        var v2 = new Vector3D(0, 1, 0);

        var result = v1 ^ v2;

        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
        Assert.Equal(1, result.Z);
    }

    [Fact]
    public void CrossProduct_IsAntisymmetric()
    {
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        var first = v1 ^ v2;
        var second = v2 ^ v1;

        Assert.Equal(-first.X, second.X);
        Assert.Equal(-first.Y, second.Y);
        Assert.Equal(-first.Z, second.Z);
    }

    [Fact]
    public void CrossProduct_IsOrthogonalToBothFactors()
    {
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        var cross = v1 ^ v2;

        Assert.Equal(0, cross * v1);
        Assert.Equal(0, cross * v2);
    }

    [Fact]
    public void Addition_ReturnsSumOfVectors()
    {
        var v1 = new Vector3D(1, 2, 3);
        var v2 = new Vector3D(4, 5, 6);

        var result = v1 + v2;

        Assert.Equal(5, result.X);
        Assert.Equal(7, result.Y);
        Assert.Equal(9, result.Z);
    }

    [Fact]
    public void Subtraction_ReturnsDifferenceOfVectors()
    {
        var v1 = new Vector3D(4, 6, 8);
        var v2 = new Vector3D(1, 2, 3);

        var result = v1 - v2;

        Assert.Equal(3, result.X);
        Assert.Equal(4, result.Y);
        Assert.Equal(5, result.Z);
    }

    [Fact]
    public void ScalarMultiplication_ReturnsScaledVector()
    {
        var vector = new Vector3D(1, 2, 3);

        var result = 2 * vector;

        Assert.Equal(2, result.X);
        Assert.Equal(4, result.Y);
        Assert.Equal(6, result.Z);
    }

    [Fact]
    public void ZeroVector_HasZeroNorm()
    {
        var vector = new Vector3D(0, 0, 0);

        Assert.Equal(0, vector.Norm());
    }
}