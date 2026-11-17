using Geometry3D.Core;
using Xunit;

namespace Geometry3D.Tests;

public class Segment3DTests
{
    [Fact]
    public void Length_ReturnsCorrectDistance()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(2, 3, 6));

        Assert.Equal(7, segment.Length());
    }

    [Fact]
    public void PointAt_ReturnsMidpoint()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 2, 6));

        var result = segment.PointAt(0.5);

        Assert.Equal(2, result.X);
        Assert.Equal(1, result.Y);
        Assert.Equal(3, result.Z);
    }

    [Fact]
    public void PointAt_Zero_ReturnsStart()
    {
        var start = new Point3D(1, 2, 3);
        var segment = new Segment3D(start, new Point3D(4, 5, 6));

        var result = segment.PointAt(0);

        Assert.Equal(start.X, result.X);
        Assert.Equal(start.Y, result.Y);
        Assert.Equal(start.Z, result.Z);
    }

    [Fact]
    public void PointAt_One_ReturnsEnd()
    {
        var end = new Point3D(4, 5, 6);
        var segment = new Segment3D(new Point3D(1, 2, 3), end);

        var result = segment.PointAt(1);

        Assert.Equal(end.X, result.X);
        Assert.Equal(end.Y, result.Y);
        Assert.Equal(end.Z, result.Z);
    }

    [Fact]
    public void PointAt_ThrowsForParameterOutsideRange()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(1, 0, 0));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => segment.PointAt(-0.1));

        Assert.Throws<ArgumentOutOfRangeException>(
            () => segment.PointAt(1.1));
    }

    [Fact]
    public void Contains_ReturnsTrueForPointOnSegment()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 0, 0));

        Assert.True(segment.Contains(new Point3D(2, 0, 0)));
    }

    [Fact]
    public void Contains_ReturnsFalseForPointOutsideLine()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 0, 0));

        Assert.False(segment.Contains(new Point3D(2, 1, 0)));
    }

    [Fact]
    public void Contains_ReturnsFalseForCollinearPointOutsideSegment()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 0, 0));

        Assert.False(segment.Contains(new Point3D(5, 0, 0)));
    }

    [Fact]
    public void Contains_HandlesDegenerateSegment()
    {
        var point = new Point3D(1, 2, 3);
        var segment = new Segment3D(point, point);

        Assert.True(segment.Contains(new Point3D(1, 2, 3)));
        Assert.False(segment.Contains(new Point3D(4, 5, 6)));
    }

    [Fact]
    public void ClosestPointTo_ReturnsProjectionInsideSegment()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 0, 0));

        var result = segment.ClosestPointTo(new Point3D(2, 3, 1));

        Assert.Equal(2, result.X);
        Assert.Equal(0, result.Y);
        Assert.Equal(0, result.Z);
    }

    [Fact]
    public void ClosestPointTo_ClampsToNearestEndpoint()
    {
        var segment = new Segment3D(
            new Point3D(0, 0, 0),
            new Point3D(4, 0, 0));

        var result = segment.ClosestPointTo(new Point3D(6, 2, 0));

        Assert.Equal(4, result.X);
        Assert.Equal(0, result.Y);
        Assert.Equal(0, result.Z);
    }

    [Fact]
    public void ClosestPointTo_ReturnsStartForDegenerateSegment()
    {
        var start = new Point3D(1, 2, 3);
        var segment = new Segment3D(start, start);

        var result = segment.ClosestPointTo(new Point3D(4, 5, 6));

        Assert.Equal(1, result.X);
        Assert.Equal(2, result.Y);
        Assert.Equal(3, result.Z);
    }
}