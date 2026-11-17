namespace Geometry3D.Core;

public class Circle
{
    public double Radius { get; }

    public Circle(double radius)
    {
        if (!double.IsFinite(radius) || radius < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(radius),
                "Il raggio deve essere finito e non negativo."
            );
        }

        Radius = radius;
    }

    public double Area()
    {
        return Math.PI * Radius * Radius;
    }

    public double Circumference()
    {
        return 2 * Math.PI * Radius;
    }
}