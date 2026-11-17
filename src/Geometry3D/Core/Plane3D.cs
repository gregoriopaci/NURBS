namespace Geometry3D.Core;

public class Plane3D
{
    // Properties per rappresentare un piano nello spazio 3D.
    // Un piano è definito da un punto e da un vettore normale.
    public Point3D Origin { get; }
    public Vector3D Normal { get; }

    private const double Tolerance = 1e-12;

    // Costruttore per inizializzare le proprietà del piano.
    public Plane3D(Point3D origin, Vector3D normal)
    {
        Origin = origin;
        Normal = normal;
    }

    // Metodo per calcolare la distanza tra un punto e il piano.
    // La distanza è positiva se il punto si trova dalla parte della normale, negativa altrimenti.
    public double SignedDistanceTo(Point3D point)
    {
        Vector3D originToPoint = point - Origin;
        return (originToPoint * Normal) / Normal.Norm();
    }

    // Metodo per verificare se un punto appartiene al piano.
    public bool Contains(Point3D point)
    {
        return Math.Abs(SignedDistanceTo(point)) <= Tolerance;
    }

    // Metodo per proiettare un punto sul piano.
    public Point3D Project(Point3D point)
    {
        Vector3D v = point - Origin;
        double normalSquared = Normal * Normal;

        if (normalSquared < Tolerance)
            throw new InvalidOperationException("Il vettore normale al piano è nullo.");

        double distanceAlongNormal = (v * Normal) / normalSquared;
        return point - distanceAlongNormal * Normal;
    }

    // Punto del piano più vicino a un punto dato.
    public Point3D ClosestPointTo(Point3D point)
    {
        return Project(point);
    }


    public double Determinant3x3(
            double a11, double a12, double a13,
            double a21, double a22, double a23,
            double a31, double a32, double a33)
    {
        return a11 * (a22 * a33 - a23 * a32)
             - a12 * (a21 * a33 - a23 * a31)
             + a13 * (a21 * a32 - a22 * a31);
    }


    // ( P - O ) * N = 0
    // O1 + t * D

    // ( O1 + t * D - O ) * N = 0
    // (O1 - O ) * N + t D * N = 0 => t = - [ (O1 - O ) * N ] /  ( D * N )

    // Intersezione tra un piano e una retta.
    public bool TryIntersectLinePlane(Line3D other, out Point3D intersection)
    {
        intersection = new Point3D(0, 0, 0);

        double scalarProdNormalDirection = Normal * other.Direction;

        // In questo caso la retta è parallela al piano.
        if (Math.Abs(scalarProdNormalDirection) < Tolerance)
        {
            return false;
        }

        Vector3D planeToLineOrigin = other.Origin - Origin;
        double t = -1 * (planeToLineOrigin * Normal) / scalarProdNormalDirection;

        intersection = other.PointAt(t);
        return true;
    }


    public enum PlaneIntersectionType
    {
        Parallel,
        Line,
        Coincident
    }

    // Intersezione tra due piani.
    // Restituisce un enum per distinguere tra:
    // - piani paralleli e distinti,
    // - piani coincidenti,
    // - piani che si intersecano lungo una retta.
    public PlaneIntersectionType TryIntersectPlanePlane(Plane3D other, out Line3D intersection, out Plane3D planeIntersection)
    {
        intersection = new Line3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 0));
        planeIntersection = new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 0));

        Vector3D direction = Normal ^ other.Normal;
        double crossNorm = direction.Norm();

        if (crossNorm < Tolerance)
        {
            if (Math.Abs(SignedDistanceTo(other.Origin)) <= Tolerance)
            {
                planeIntersection = new Plane3D(other.Origin, other.Normal);
                return PlaneIntersectionType.Coincident;
            }

            return PlaneIntersectionType.Parallel;
        }


        /* Se il prodotto vettoriale non e' nullo, i piani si intersecano lungo una retta che 
           deve appartenere a entrambi i piani (direzione normale ad entrambi i piani). 
           Questo è sufficiente per definire la direzione della retta di intersezione: d = n ^ m. 
           Ora devo trovare un punto O che appartenga alla retta e a entrambi i piani. 
           (O - Op) * n = 0 e (O - Oq) * m = 0 ---> ( O * n = Op * n && O * m = Oq * m && O * d = 0 ) 
           l'ultima condizione è necessaria per trovare un punto unico. 
           Come condizione aggiuntiva scelgo O * d = 0, 
           cioè cerco il punto della retta di intersezione più vicino all'origine. */


        // Risolvo il sistema con cramer.    

        double c1 = Origin * Normal;
        double c2 = other.Origin * other.Normal;
        double det = Determinant3x3(
            Normal.X, Normal.Y, Normal.Z,
            other.Normal.X, other.Normal.Y, other.Normal.Z,
            direction.X, direction.Y, direction.Z);

        if (Math.Abs(det) < Tolerance)
        {
            return PlaneIntersectionType.Parallel;
        }

        double x = Determinant3x3(
            c1, Normal.Y, Normal.Z,
            c2, other.Normal.Y, other.Normal.Z,
            0, direction.Y, direction.Z) / det;

        double y = Determinant3x3(
            Normal.X, c1, Normal.Z,
            other.Normal.X, c2, other.Normal.Z,
            direction.X, 0, direction.Z) / det;

        double z = Determinant3x3(
            Normal.X, Normal.Y, c1,
            other.Normal.X, other.Normal.Y, c2,
            direction.X, direction.Y, 0) / det;

        Point3D pointOnLine = new Point3D(x, y, z);
        intersection = new Line3D(pointOnLine, direction);
        return PlaneIntersectionType.Line;
    }




}