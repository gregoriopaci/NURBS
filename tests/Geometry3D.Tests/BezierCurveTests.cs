using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class BezierCurveTests
{
    private static void AssertPointEqual(
        Point3D expected, Point3D actual, int precision = 10)
    {
        Assert.Equal(expected.X, actual.X, precision);
        Assert.Equal(expected.Y, actual.Y, precision);
        Assert.Equal(expected.Z, actual.Z, precision);
    }

    [Fact]
    public void Constructor_SetsDegree()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 2, 0),
            new Point3D(2, 0, 0)
        });

        Assert.Equal(2, curve.Degree);
    }

    [Fact]
    public void Constructor_RejectsNullControlPoints()
    {
        Assert.Throws<ArgumentNullException>(
            () => new BezierCurve(null!));
    }

    [Fact]
    public void Constructor_RejectsNullPoint()
    {
        var points = new Point3D[]
        {
            new(0, 0, 0),
            null!
        };

        Assert.Throws<ArgumentException>(
            () => new BezierCurve(points));
    }   

[Theory]
[InlineData(double.NaN)]
[InlineData(double.PositiveInfinity)]
[InlineData(double.NegativeInfinity)]
[InlineData(0.0)]
[InlineData(-1.0)]
public void Constructor_RejectsInvalidWeights(double weight)
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    };

    Assert.Throws<ArgumentException>(
        () => new BezierCurve(points, new[] { 1.0, weight }));
}

[Fact]
public void Constructor_RejectsMismatchedWeights()
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    };

    Assert.Throws<ArgumentException>(
        () => new BezierCurve(points, new[] { 1.0 }));
}

    [Fact]
    public void Evaluate_ReturnsFirstControlPointAtZero()
    {
        var p0 = new Point3D(1, 2, 3);
        var curve = new BezierCurve(new[]
        {
            p0,
            new Point3D(4, 5, 6)
        });

        AssertPointEqual(p0, curve.Evaluate(0));
    }

    [Fact]
    public void Evaluate_ReturnsLastControlPointAtOne()
    {
        var p1 = new Point3D(4, 5, 6);
        var curve = new BezierCurve(new[]
        {
            new Point3D(1, 2, 3),
            p1
        });

        AssertPointEqual(p1, curve.Evaluate(1));
    }

    [Fact]
    public void Evaluate_LinearCurveReturnsMidpoint()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(4, 2, 6)
        });

        AssertPointEqual(
            new Point3D(2, 1, 3),
            curve.Evaluate(0.5));
    }

    [Fact]
    public void Evaluate_MatchesBernsteinForPolynomialCurve()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(2, 4, 0),
            new Point3D(4, 0, 0)
        });

        AssertPointEqual(
            curve.EvaluateBernstein(0.3),
            curve.Evaluate(0.3));
    }

    [Fact]
    public void Evaluate_RationalQuarterCircle()
    {
        double w = Math.Sqrt(2) / 2;

        var curve = new BezierCurve(
            new[]
            {
                new Point3D(1, 0, 0),
                new Point3D(1, 1, 0),
                new Point3D(0, 1, 0)
            },
            new[] { 1.0, w, 1.0 });

        double coordinate = Math.Sqrt(2) / 2;

        AssertPointEqual(
            new Point3D(coordinate, coordinate, 0),
            curve.Evaluate(0.5));
    }

    [Fact]
    public void IsRational_ReturnsFalseForUnitWeights()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 1, 0)
        });

        Assert.False(curve.IsRational);
    }

    [Fact]
    public void IsRational_ReturnsTrueForNonUnitWeights()
    {
        var curve = new BezierCurve(
            new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 1, 0)
            },
            new[] { 1.0, 2.0 });

        Assert.True(curve.IsRational);
    }

    [Fact]
    public void Split_PreservesOriginalCurve()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(2, 4, 0),
            new Point3D(4, 0, 0)
        });

        double splitParameter = 0.4;
        var (left, right) = curve.Split(splitParameter);

        // La curva sinistra deve corrispondere
        // al primo tratto della curva originale.
        for (int i = 0; i <= 10; i++)
        {
            double u = i / 10.0;

            AssertPointEqual(
                curve.Evaluate(u * splitParameter),
                left.Evaluate(u));
        }

        // La curva destra deve corrispondere
        // al secondo tratto della curva originale.
        for (int i = 0; i <= 10; i++)
        {
            double u = i / 10.0;

            AssertPointEqual(
                curve.Evaluate(
                    splitParameter + u * (1 - splitParameter)),
                right.Evaluate(u));
        }
    }

    [Fact]
    public void Split_PreservesRationalCurve()
    {
        double w = Math.Sqrt(2) / 2;

        var curve = new BezierCurve(
            new[]
            {
                new Point3D(1, 0, 0),
                new Point3D(1, 1, 0),
                new Point3D(0, 1, 0)
            },
            new[] { 1.0, w, 1.0 });

        var (left, right) = curve.Split(0.5);

        Assert.True(left.IsRational);
        Assert.True(right.IsRational);

        AssertPointEqual(curve.Evaluate(0.5), left.Evaluate(1));
        AssertPointEqual(curve.Evaluate(0.5), right.Evaluate(0));
    }

    [Fact]
    public void Derivative_ReturnsCorrectLinearDerivative()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(4, 2, 6)
        });

        var derivative = curve.Derivative(0.5);

        Assert.Equal(4, derivative.X, 10);
        Assert.Equal(2, derivative.Y, 10);
        Assert.Equal(6, derivative.Z, 10);
    }

    [Fact]
    public void Derivative_MatchesFiniteDifference()
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(2, 4, 1),
            new Point3D(5, 1, 3)
        });

        double t = 0.4;
        double h = 1e-6;

        var before = curve.Evaluate(t - h);
        var after = curve.Evaluate(t + h);

        var numericalDerivative =
            (after - before) * (1.0 / (2 * h));

        var analyticalDerivative = curve.Derivative(t);

        Assert.Equal(numericalDerivative.X, analyticalDerivative.X, 5);
        Assert.Equal(numericalDerivative.Y, analyticalDerivative.Y, 5);
        Assert.Equal(numericalDerivative.Z, analyticalDerivative.Z, 5);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    public void Evaluate_RejectsInvalidParameter(double t)
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 1, 0)
        });

        Assert.Throws<ArgumentOutOfRangeException>(
            () => curve.Evaluate(t));
    }


    [Theory]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void EvaluateBernstein_RejectsInvalidParameter(double t)
    {
        var curve = new BezierCurve(new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 1, 0)
        });

        Assert.Throws<ArgumentOutOfRangeException>(
            () => curve.EvaluateBernstein(t));
    }

[Theory]
[InlineData(-0.1)]
[InlineData(1.1)]
[InlineData(double.NaN)]
[InlineData(double.PositiveInfinity)]
[InlineData(double.NegativeInfinity)]
public void Split_RejectsInvalidParameter(double t)
{
    var curve = new BezierCurve(new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    });

    Assert.Throws<ArgumentOutOfRangeException>(
        () => curve.Split(t));
}

[Theory]
[InlineData(-0.1)]
[InlineData(1.1)]
[InlineData(double.NaN)]
[InlineData(double.PositiveInfinity)]
[InlineData(double.NegativeInfinity)]
public void Derivative_RejectsInvalidParameter(double t)
{
    var curve = new BezierCurve(new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    });

    Assert.Throws<ArgumentOutOfRangeException>(
        () => curve.Derivative(t));
}
}