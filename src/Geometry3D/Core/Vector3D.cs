namespace Geometry3D.Core;

public class Vector3D
{
    // Properties di Vector3D
    public double X { get; set; }
    public double Y { get; set; }
    public double Z { get; set; }

    // Costrutture
    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Norma di un vettore
    public double Norm()
    {
        return Math.Sqrt(X * X + Y * Y + Z * Z);
    }

    // Somma di un vettori senza overload
    public Vector3D Sum(Vector3D other)
    {
        return new Vector3D(other.X + X, other.Y + Y, other.Z + Z);
    }

    // Scalar product vettori senza overload
    public double Dot(Vector3D other)
    {
        return other.X * X + other.Y * Y + other.Z * Z;
    }

    // Cross product vettori senza overload
    public Vector3D Cross(Vector3D other)
    {
        return new Vector3D(
            Y * other.Z - Z * other.Y,
            Z * other.X - X * other.Z,
            X * other.Y - Y * other.X
        );
    }

    // invece di definire somme/prodotti tra vettori come metodi, potrei ridefinire degli operatori. 
    // In questo caso definisco l'operatore + per sommare due vettori
    // Somma tra vettori
    public static Vector3D operator +(Vector3D v1, Vector3D v2)
    {
        return new Vector3D(
            v1.X + v2.X,
            v1.Y + v2.Y,
            v1.Z + v2.Z
        );
    }

    // Somma tra un vettore e un punto. Il risultato e' un nuovo punto. Questo si puo' usare per definire una 
    // retta passante per un punto e parallela a un vettore. 
    public static Point3D operator +(Point3D p, Vector3D v)
    {
        return new Point3D(
            p.X + v.X,
            p.Y + v.Y,
            p.Z + v.Z
        );
    }

    // Differenza tra vettori overload
    public static Vector3D operator -(Vector3D a, Vector3D b)
    {
        return new Vector3D(
            a.X - b.X,
            a.Y - b.Y,
            a.Z - b.Z
        );
    }

    // Differenza di uno punto e  un vettore overload.
    public static Point3D operator -(Point3D p, Vector3D v)
    {
        return new Point3D(
            p.X - v.X,
            p.Y - v.Y,
            p.Z - v.Z
        );
    }

    // Prodotto scalare tra vettori overload
    public static double operator *(Vector3D a, Vector3D b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    // Prodotto scalare tra points e vectors overload
    public static double operator *(Point3D a, Vector3D b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }

    // Prodotto vettoriale tra vettori overload. 
    // Lo definisco per mezzo dell'operatore ^ che di solito è lo XOR
    public static Vector3D operator ^(Vector3D a, Vector3D b)
    {
        return new Vector3D(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );

    }

    // Prodotto di un vettore per uno scalare overload.
    public static Vector3D operator *(Vector3D a, double scalar)
    {
        return new Vector3D(
            a.X * scalar,
            a.Y * scalar,
            a.Z * scalar
        );
    }

    // Prodotto di uno scalare per un vettore overload.
    public static Vector3D operator *(double scalar, Vector3D a)
    {
        return new Vector3D(
            a.X * scalar,
            a.Y * scalar,
            a.Z * scalar
        );
    }


}