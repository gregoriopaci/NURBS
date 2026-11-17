
using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class BSplineCurveTests
{
    private static void AssertPointEqual(
        Point3D expected,
        Point3D actual,
        int precision = 10)
    {
        Assert.Equal(expected.X, actual.X, precision);
        Assert.Equal(expected.Y, actual.Y, precision);
        Assert.Equal(expected.Z, actual.Z, precision);
    }

    private static BSplineCurve CreateLinearCurve()
    {
        var points = new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 2, 0),
            new Point3D(2, 0, 0)
        };

        return new BSplineCurve(
            points,
            1,
            BSplineCurve.CreateOpenUniformKnots(3, 1));
    }

    [Fact]
    public void Constructor_SetsProperties()
    {
        var curve = CreateLinearCurve();

        Assert.Equal(1, curve.Degree);
        Assert.Equal(0.0, curve.DomainStart);
        Assert.Equal(1.0, curve.DomainEnd);
        Assert.Equal(3, curve.ControlPoints.Count);
        Assert.Equal(5, curve.Knots.Count);
    }

    [Fact]
public void Constructor_RejectsNullControlPoints()
{
    Assert.Throws<ArgumentNullException>(() =>
        new BSplineCurve(null!, 1, new double[] { 0, 0, 1, 1 }));
}

[Fact]
public void Constructor_RejectsNullKnots()
{
    Assert.Throws<ArgumentNullException>(() =>
        new BSplineCurve(
            new[] { new Point3D(0, 0, 0), new Point3D(1, 1, 1) },
            1,
            null!));
}

[Fact]
public void Constructor_RejectsIncorrectKnotCount()
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0),
        new Point3D(2, 0, 0)
    };

    Assert.Throws<ArgumentException>(() =>
        new BSplineCurve(points, 1, new double[] { 0, 0, 1 }));
}

[Theory]
[InlineData(double.NaN)]
[InlineData(double.PositiveInfinity)]
[InlineData(double.NegativeInfinity)]
public void Constructor_RejectsNonFiniteKnots(double invalidKnot)
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    };

    Assert.Throws<ArgumentException>(() =>
        new BSplineCurve(points, 1,
            new[] { 0.0, 0.0, invalidKnot, 1.0 }));
}

[Fact]
public void Constructor_RejectsDecreasingKnots()
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0),
        new Point3D(2, 0, 0)
    };

    Assert.Throws<ArgumentException>(() =>
        new BSplineCurve(points, 1,
            new double[] { 0, 0, 0.7, 0.5, 1 }));
}

[Fact]
public void Constructor_RejectsZeroWidthDomain()
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    };

    Assert.Throws<ArgumentException>(() =>
        new BSplineCurve(points, 1,
            new double[] { 0, 0, 0, 0 }));
}

[Theory]
[InlineData(0)]
[InlineData(-1)]
[InlineData(2)]
public void Constructor_RejectsInvalidDegree(int degree)
{
    var points = new[]
    {
        new Point3D(0, 0, 0),
        new Point3D(1, 1, 0)
    };

    Assert.Throws<ArgumentOutOfRangeException>(() =>
        new BSplineCurve(points, degree,
            new double[] { 0, 0, 1, 1 }));
}

[Fact]
public void Constructor_RejectsNullControlPointEntries()
{
    var points = new Point3D[]
    {
        new(0, 0, 0),
        null!
    };

    Assert.Throws<ArgumentException>(() =>
        new BSplineCurve(points, 1, new double[] { 0, 0, 1, 1 }));
}

    [Fact]
    public void Evaluate_ReturnsFirstControlPointAtDomainStart()
    {
        var curve = CreateLinearCurve();

        AssertPointEqual(
            curve.ControlPoints[0],
            curve.Evaluate(curve.DomainStart));
    }

    [Fact]
    public void Evaluate_ReturnsLastControlPointAtDomainEnd()
    {
        var curve = CreateLinearCurve();

        AssertPointEqual(
            curve.ControlPoints[^1],
            curve.Evaluate(curve.DomainEnd));
    }

    [Fact]
    public void Evaluate_LinearCurveAtInternalKnot()
    {
        var curve = CreateLinearCurve();

        var result = curve.Evaluate(0.5);

        AssertPointEqual(
            new Point3D(1, 2, 0),
            result);
    }

    [Fact]
    public void Evaluate_InterpolatesBetweenControlPoints()
    {
        var curve = CreateLinearCurve();

        var result = curve.Evaluate(0.25);

        AssertPointEqual(
            new Point3D(0.5, 1, 0),
            result);
    }

    [Fact]
    public void Evaluate_RejectsParametersOutsideDomain()
    {
        var curve = CreateLinearCurve();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => curve.Evaluate(-0.1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => curve.Evaluate(1.1));
    }

    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void Evaluate_RejectsNonFiniteParameters(double t)
    {
        var curve = CreateLinearCurve();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => curve.Evaluate(t));
    }

    [Fact]
    public void CreateOpenUniformKnots_GeneratesCorrectVector()
    {
        var knots = BSplineCurve.CreateOpenUniformKnots(5, 2);

        Assert.Equal(
            new[] { 0.0, 0.0, 0.0, 1.0 / 3.0, 2.0 / 3.0, 1.0, 1.0, 1.0 },
            knots);
    }

    [Fact]
    public void CreateOpenUniformKnots_RejectsInvalidArguments()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => BSplineCurve.CreateOpenUniformKnots(1, 1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => BSplineCurve.CreateOpenUniformKnots(3, 0));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => BSplineCurve.CreateOpenUniformKnots(3, 3));
    }

    [Fact]
    public void Evaluate_HandlesRepeatedInternalKnots()
    {
        var points = new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 1, 0),
            new Point3D(2, 0, 0),
            new Point3D(3, 1, 0)
        };

        var knots = new[] { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0 };

        var curve = new BSplineCurve(points, 2, knots);

        var result = curve.Evaluate(0.5);

        Assert.True(double.IsFinite(result.X));
        Assert.True(double.IsFinite(result.Y));
        Assert.True(double.IsFinite(result.Z));
    }
}
