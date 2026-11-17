namespace Geometry3D.Core;


public class Segment3D
{
    //Properties Segments
    public Point3D Start { get; }
    public Point3D End { get; }

    public Vector3D Direction
    {
        get
        {
            return End - Start;
        }
    }

    private const double Tolerance = 1e-12;

    // Costruttore per inizializzare le proprieta' del segmento
    public Segment3D(Point3D start, Point3D end)
    {
        Start = start;
        End = end;
    }


    // Lunghezza del segmento
    public double Length()
    {
        return (End - Start).Norm();
    }

    // Punto parametrico sul segmento
    // t deve essere compreso tra 0 e 1
    public Point3D PointAt(double t)
    {
        if (t < 0 || t > 1)
        {
            throw new ArgumentOutOfRangeException(
                "t",
                "Il parametro deve essere compreso tra 0 e 1."
            );
        }

        return Start + t * Direction;
    }


    // Verifica se un punto appartiene al segmento
    public bool Contains(Point3D point)
    {
        double lengthSquared = Direction * Direction;

        // Segmento degenere: coincide con un singolo punto.
        if (lengthSquared < Tolerance)
            return Start.DistanceTo(point) <= Tolerance;

        // Verifico che il punto appartenga alla retta contenente il segmento.
        Vector3D startToPoint = point - Start;
        Vector3D cross = startToPoint ^ Direction;

        if (cross.Norm() > Tolerance)
            return false;

        // Verifico che il punto si trovi tra Start ed End.
        double dot = startToPoint * Direction;

        return dot >= -Tolerance &&
            dot <= lengthSquared + Tolerance;
    }


    // Punto più vicino sul segmento a un punto esterno
    public Point3D ClosestPointTo(Point3D point)
    {
        Vector3D startToPoint = point - Start;

        double lengthSquared = Direction * Direction;

        if (lengthSquared < Tolerance)
            return Start; // segmento degenerato


        // parametro sulla retta infinita
        double t = (startToPoint * Direction) / lengthSquared;


        // differenza fondamentale rispetto a Line3D:
        // qui limito t tra 0 e 1
        t = Math.Max(0, Math.Min(1, t));


        return PointAt(t);
    }
}