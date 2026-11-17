using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class Line3DTests
{
    [Fact]
    public void PointAt_ReturnsCorrectPoint()
    {
        var line = new Line3D(
            new Point3D(1, 2, 3),
            new Vector3D(2, 0, -1));

        var result = line.PointAt(2);

        Assert.Equal(5, result.X);
        Assert.Equal(2, result.Y);
        Assert.Equal(1, result.Z);
    }

    [Fact]
    public void ClosestPointTo_ReturnsOrthogonalProjection()
    {
        var line = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var result = line.ClosestPointTo(new Point3D(2, 3, 4));

        Assert.Equal(2, result.X);
        Assert.Equal(0, result.Y);
        Assert.Equal(0, result.Z);
    }

    [Fact]
    public void Contains_ReturnsTrueForPointOnLine()
    {
        var line = new Line3D(
            new Point3D(1, 2, 3),
            new Vector3D(2, 1, 0));

        var point = line.PointAt(3);

        Assert.True(line.Contains(point));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointOutsideLine()
    {
        var line = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        Assert.False(line.Contains(new Point3D(1, 1, 0)));
    }

    [Fact]
    public void TryClosestPoints_ReturnsCorrectPointsForIntersectingLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(1, -1, 0),
            new Vector3D(0, 1, 0));

        bool result = line1.TryClosestPoints(
            line2,
            out var point1,
            out var point2,
            out double distance);

        Assert.True(result);

        Assert.Equal(1, point1.X);
        Assert.Equal(0, point1.Y);
        Assert.Equal(0, point1.Z);

        Assert.Equal(1, point2.X);
        Assert.Equal(0, point2.Y);
        Assert.Equal(0, point2.Z);

        Assert.Equal(0, distance);
    }

    [Fact]
    public void TryClosestPoints_ReturnsCorrectDistanceForSkewLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(0, 1, 1),
            new Vector3D(0, 1, 0));

        bool result = line1.TryClosestPoints(
            line2,
            out var point1,
            out var point2,
            out double distance);

        Assert.True(result);
        Assert.Equal(0, point1.X);
        Assert.Equal(0, point1.Y);
        Assert.Equal(0, point1.Z);

        Assert.Equal(0, point2.X);
        Assert.Equal(0, point2.Y);
        Assert.Equal(1, point2.Z);

        Assert.Equal(1, distance);
    }

    [Fact]
    public void TryClosestPoints_ReturnsFalseForParallelLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(0, 2, 0),
            new Vector3D(2, 0, 0));

        bool result = line1.TryClosestPoints(
            line2,
            out var point1,
            out var point2,
            out double distance);

        Assert.False(result);
        Assert.Equal(2, distance);
        Assert.Equal(2, point1.DistanceTo(point2));
    }

    [Fact]
    public void DistanceTo_ReturnsMinimumDistanceBetweenLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(0, 1, 1),
            new Vector3D(0, 1, 0));

        Assert.Equal(1, line1.DistanceTo(line2));
    }

    [Fact]
    public void TryIntersect_ReturnsIntersectionForIntersectingLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(1, -1, 0),
            new Vector3D(0, 1, 0));

        bool result = line1.TryIntersect(line2, out var intersection);

        Assert.True(result);
        Assert.Equal(1, intersection.X);
        Assert.Equal(0, intersection.Y);
        Assert.Equal(0, intersection.Z);
    }

    [Fact]
    public void TryIntersect_ReturnsFalseForParallelLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(0, 2, 0),
            new Vector3D(1, 0, 0));

        bool result = line1.TryIntersect(line2, out var intersection);

        Assert.False(result);
        Assert.Equal(0, intersection.X);
        Assert.Equal(0, intersection.Y);
        Assert.Equal(0, intersection.Z);
    }

    [Fact]
    public void TryIntersect_ReturnsFalseForSkewLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(0, 1, 1),
            new Vector3D(0, 1, 0));

        bool result = line1.TryIntersect(line2, out _);

        Assert.False(result);
    }

    [Fact]
    public void TryIntersect_ReturnsFalseForCoincidentLines()
    {
        var line1 = new Line3D(
            new Point3D(0, 0, 0),
            new Vector3D(1, 0, 0));

        var line2 = new Line3D(
            new Point3D(2, 0, 0),
            new Vector3D(2, 0, 0));

        bool result = line1.TryIntersect(line2, out _);

        Assert.False(result);
    }
}