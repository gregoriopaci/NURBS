namespace Geometry3D.Core;

public class Line3D
{
    // Properties per rappresentare una retta nello spazio 3D. 
    // Una retta e' definita da un punto e da un vettore direzione
    public Point3D Origin { get; }
    public Vector3D Direction { get; }

    // Costante numericamente zero.
    private const double Tolerance = 1e-12;


    // Costruttore per inizializzare le proprieta' della retta
    public Line3D(Point3D origin, Vector3D direction)
    {
        Origin = origin;
        Direction = direction;
    }

    // Metodo per calcolare un punto sulla retta dato un parametro t. 
    // Il punto e' dato dalla formula: P(t) = Origin + t * Direction
    public Point3D PointAt(double t)
    {
        return Origin + t * Direction;
    }

    // Questo metodo trova il punto sulla retta piu' vicino a un punto dato
    // La condizione di minimo e': (Point - Origin - t * Direction) * Direction = 0 
    public Point3D ClosestPointTo(Point3D point)
    {
        Vector3D originToPoint = point - Origin;
        double directionSquared = Direction * Direction;

        if (directionSquared < Tolerance)
        {
            return Origin;
        }

        double t = (originToPoint * Direction) / directionSquared;
        return PointAt(t);
    }

    // Uso ClosestPointTo per verificare se un punto appartiene alla retta. 
    // Se la distanza tra il punto e il punto piu vicino sulla retta e' minore di Tolerance, 
    // allora il punto appartiene alla retta.
    public bool Contains(Point3D point)
    {
        Point3D closest = ClosestPointTo(point);
        return closest.DistanceTo(point) <= Tolerance;
    }

    // Metodo per calcolare i punti a distanza minima tra due rette L1 = O1 + t * d1,  L2 = O2 + s * d2.
    // Restituisce true se le rette non sono parallele; se sono parallele restituisce comunque i punti
    // corrispondenti alla distanza minima (linee parallele hanno infinite coppie equivalenti).
    // t ed s sono trovati usando le condizioni di minimo  ( (L1 - L2) * d1 = 0 && (L1 - L2) * d2 = 0 )
    // L1 - L2 = O1 - O2 + t * d1 - s * d2 = r  + t * d1  - s * d2
    // 1st condition --> r * d1 + t * d1d1 - s * d2d1 = 0 --> - t * d1d1 + s * d1d2 = rD1
    // 2nd condition --> r * d2 + t * d1d2 - s * d2d2 = 0 --> - t * d1d2 + s * d2d2 = rD2
    public bool TryClosestPoints(Line3D other, out Point3D pointOnThis, out Point3D pointOnOther, out double minimumDistance)
    {
        pointOnThis = new Point3D(0, 0, 0);
        pointOnOther = new Point3D(0, 0, 0);
        minimumDistance = double.PositiveInfinity;

        double d1d1 = Direction * Direction;
        double d2d2 = other.Direction * other.Direction;
        double d1d2 = Direction * other.Direction;
        Vector3D r = Origin - other.Origin;
        double rD1 = r * Direction;
        double rD2 = r * other.Direction;

        double det = -d1d1 * d2d2 + d1d2 * d1d2;
        if (Math.Abs(det) < Tolerance)
        {
            // Retta parallela: scegliamo Origin come punto sulla prima retta
            // e il punto piu' vicino sulla seconda retta rispetto a Origin.
            pointOnThis = Origin;
            pointOnOther = other.ClosestPointTo(Origin);
            minimumDistance = pointOnThis.DistanceTo(pointOnOther);
            return false;
        }

        // Soluzione con la regola di Cramer per il sistema di equazioni lineari.
        double t = (rD1 * d2d2 - rD2 * d1d2) / det;
        double s = (rD1 * d1d2 - rD2 * d1d1) / det;

        pointOnThis = PointAt(t);
        pointOnOther = other.PointAt(s);
        minimumDistance = pointOnThis.DistanceTo(pointOnOther);
        return true;
    }

    // Metodo per calcolare la distanza minima tra due rette nello spazio 3D.
    public double DistanceTo(Line3D other)
    {
        TryClosestPoints(other, out Point3D pointOnThis, out Point3D pointOnOther, out double minimumDistance);
        return minimumDistance;
    }

    // Metodo per trovare l'intersezione tra due rette nello spazio 3D. 
    // Se le rette si intersecano, restituisce true e riempie out intersection.
    // Se sono parallele o skew, restituisce false e (0,0,0) come intersection point.
    // out indica che Point3D intersection e' un parametro di output che 
    // verra' assegnato all'interno del metodo.
    public bool TryIntersect(Line3D other, out Point3D intersection)
    {
        intersection = new Point3D(0, 0, 0);

        Vector3D crossDirection = Direction ^ other.Direction;
        double crossNorm = crossDirection.Norm();

        // Se le direzioni sono parallele o quasi parallele, 
        // non c'e' un punto di intersezione unico
        if (crossNorm < Tolerance)
        {
            return false;
        }

        // Sfrutto l'overload degli operator che ho definito nelle classi
        // Vector3D e Point3D
        Vector3D originDifference = other.Origin - Origin;
        double crossNormSquared = crossNorm * crossNorm;


        // L1 = O1 + t * d1,  L2 = O2 + s * d2. 
        // Cerco O1 + t * d1 = O2 + s * d2 --> O1 - O2 + t * d1 -  s * d2 = 0
        // (O1 - O2) ^ d1 - s * d2 ^ d1 = 0 --> (O12 ^ d1) * (d1 ^ d2) + s * (d1 ^ d2) * (d1 ^ d2) = 0
        // (O1 - O2) ^ d2 + t * d1 ^ d2 = 0 --> (O12 ^ d2) * (d1 ^ d2) + t * (d1 ^ d2) * (d1 ^ d2) = 0
        double t = (originDifference ^ other.Direction) * crossDirection / crossNormSquared;
        double s = (originDifference ^ Direction) * crossDirection / crossNormSquared;

        Point3D pointOnThis = PointAt(t);
        Point3D pointOnOther = other.PointAt(s);

        // L'intersezione esiste se e solo se i due punti sulla retta piu' vicini tra loro coincidono. 
        if (pointOnThis.DistanceTo(pointOnOther) > Tolerance)
        {
            return false;
        }

        intersection = pointOnThis;
        return true;
    }

}