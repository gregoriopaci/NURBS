using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class Plane3DTests
{
    [Fact]
    public void SignedDistanceTo_ReturnsCorrectSignedDistance()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        Assert.Equal(3, plane.SignedDistanceTo(new Point3D(1, 2, 3)));
        Assert.Equal(-2, plane.SignedDistanceTo(new Point3D(1, 2, -2)));
    }

    [Fact]
    public void Contains_ReturnsTrueForPointOnPlane()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        Assert.True(plane.Contains(new Point3D(2, 3, 0)));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointOutsidePlane()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        Assert.False(plane.Contains(new Point3D(2, 3, 1)));
    }

    [Fact]
    public void Project_ReturnsOrthogonalProjection()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var result = plane.Project(new Point3D(2, 3, 4));

        Assert.Equal(2, result.X);
        Assert.Equal(3, result.Y);
        Assert.Equal(0, result.Z);
    }

    [Fact]
    public void Project_ThrowsForZeroNormal()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 0));

        Assert.Throws<InvalidOperationException>(
            () => plane.Project(new Point3D(1, 2, 3)));
    }

    [Fact]
    public void ClosestPointTo_ReturnsSameResultAsProject()
    {
        var plane = new Plane3D(
            new Point3D(1, 0, 0),
            new Vector3D(1, 0, 0));

        var point = new Point3D(4, 2, 3);

        var projected = plane.Project(point);
        var closest = plane.ClosestPointTo(point);

        Assert.Equal(projected.X, closest.X);
        Assert.Equal(projected.Y, closest.Y);
        Assert.Equal(projected.Z, closest.Z);
    }

    [Fact]
    public void TryIntersectLinePlane_ReturnsIntersection()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var line = new Line3D(
            new Point3D(1, 2, 3),
            new Vector3D(0, 0, -1));

        bool result = plane.TryIntersectLinePlane(line, out var intersection);

        Assert.True(result);
        Assert.Equal(1, intersection.X);
        Assert.Equal(2, intersection.Y);
        Assert.Equal(0, intersection.Z);
    }

    [Fact]
    public void TryIntersectLinePlane_ReturnsFalseForParallelLine()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var line = new Line3D(
            new Point3D(0, 0, 2),
            new Vector3D(1, 0, 0));

        bool result = plane.TryIntersectLinePlane(line, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryIntersectPlanePlane_ReturnsLineForIntersectingPlanes()
    {
        var plane1 = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var plane2 = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var type = plane1.TryIntersectPlanePlane(
            plane2,
            out var intersection,
            out _);

        Assert.Equal(Plane3D.PlaneIntersectionType.Line, type);

        Assert.True(plane1.Contains(intersection.Origin));
        Assert.True(plane2.Contains(intersection.Origin));

        Assert.Equal(0, (intersection.Direction * plane1.Normal));
        Assert.Equal(0, (intersection.Direction * plane2.Normal));
    }

    [Fact]
    public void TryIntersectPlanePlane_ReturnsParallelForDistinctParallelPlanes()
    {
        var plane1 = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var plane2 = new Plane3D(
            new Point3D(0, 0, 2),
            new Vector3D(0, 0, 1));

        var type = plane1.TryIntersectPlanePlane(
            plane2,
            out _,
            out _);

        Assert.Equal(Plane3D.PlaneIntersectionType.Parallel, type);
    }

    [Fact]
    public void TryIntersectPlanePlane_ReturnsCoincidentForSamePlane()
    {
        var plane1 = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        var plane2 = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 2));

        var type = plane1.TryIntersectPlanePlane(
            plane2,
            out _,
            out _);

        Assert.Equal(Plane3D.PlaneIntersectionType.Coincident, type);
    }

    [Fact]
    public void Determinant3x3_ReturnsCorrectValue()
    {
        var plane = new Plane3D(
            new Point3D(0, 0, 0),
            new Vector3D(0, 0, 1));

        double determinant = plane.Determinant3x3(
            1, 2, 3,
            0, 1, 4,
            5, 6, 0);

        Assert.Equal(1, determinant);
    }
}