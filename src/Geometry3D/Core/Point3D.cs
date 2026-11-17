namespace Geometry3D.Core;

public class Point3D
{
    //Properties della classe Point3D: un meccanismo che permette di esporre e controllare l'accesso ai dati interni di una classe
    /*
    get e set segretamente è come se fossero due metodi pubblici come:
    public int GetX()
        {
            return x;
        }
    public void SetX(double value)
        {
            if (value >= 0)
            {
                x = value;
            }
    che potrei per esempio chiamare (solo per x )

    Person p = new Point3D();
    p.SetX(30);

    Console.WriteLine(p.GetAge());

    */
    //Nota che benché anche il costruttore assegni un valore alla cordinata, le due cose avvengo in momenti diversi
    /*
    new Point3D(3) ------> viene chiamato il COSTRUTTORE: X=3 ------>  viene chiamato il SETTER: x=3
    se  qualche riga dopo, scrivo: p.X = 10; allora 

    p.X = 10 ------>viene chiamato SOLO il setter: x=10
     Il costruttore crea l'oggetto, le sue proprieta possono venire poi modificate a piacimento con set e get senza dover creare un nuovo oggetto
    */
    private double x;
    private double y;
    private double z;

    public double X
    {
        get { return x; }
        set { x = value; }
    }

    public double Y
    {
        get { return y; }
        set { y = value; }
    }

    public double Z
    {
        get { return z; }
        set { z = value; }
    }


    // Un semplice costruttore che assegna i valori delle coordinate
    public Point3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    // Un metodo pubblico per calcolare la distanza tra due punti
    public double DistanceTo(Point3D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        double dz = Z - other.Z;

        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    // un metodo alternativo per calcolare la distanza meno OOP minded e piu' simile a Mathematica
    // Devo dichiaralo come STATIC perche' per come e' costruito non ci accedo da un'istanza ma da usando Point3D.Distance(p1,p3). 
    public static double Distance(Point3D p1, Point3D p2)
    {
        double dx = p1.X - p2.X;
        double dy = p1.Y - p2.Y;
        double dz = p1.Z - p2.Z;

        return Math.Sqrt(dx * dx + dy * dy + dz * dz);
    }

    //un metodo per sottrarre 2 punti. Il risultato e' un vettore, per cui definisco la classe Vector3D 
    public Vector3D Subtract(Point3D other)
    {
        double dx = X - other.X;
        double dy = Y - other.Y;
        double dz = Z - other.Z;

        return new Vector3D(dx, dy, dz);
    }


    //Overload operatore - per differenza tra  punti (restituisce un vettore)
    public static Vector3D operator -(Point3D a, Point3D b)
    {
        return new Vector3D(
            a.X - b.X,
            a.Y - b.Y,
            a.Z - b.Z
        );
    }

    //Overload operatore + per somma tra  punti (restituisce un point)
    public static Point3D operator +(Point3D a, Point3D b)
    {
        return new Point3D(
            a.X + b.X,
            a.Y + b.Y,
            a.Z + b.Z
        );
    }


    // Un altro metodo per far interagire la classe Point3D con la classe Vector3D. 
    // In questo caso il risultato è un nuovo punto
    public Point3D Translate(Vector3D vector)
    {
        return new Point3D(X + vector.X, Y + vector.Y, Z + vector.Z);
    }
    //Overloan operatore + sta nella classe Vector3D per sommare un punto e un vettore (restituisce un nuovo punto)
    /*public static Point3D operator +(Point3D p, Vector3D v)
    {
        return new Point3D(
            p.X + v.X,
            p.Y + v.Y,
            p.Z + v.Z
        );
    }*/


    // Linear interpolation between two points. Lo useremo per gli algoritmi di De Casteljuau e Cox-de Boor.
    public static Point3D Lerp(Point3D p0, Point3D p1, double t)
    {
        return new Point3D(
            p0.X * (1 - t) + p1.X * t,
            p0.Y * (1 - t) + p1.Y * t,
            p0.Z * (1 - t) + p1.Z * t
        );
    }

    //Overload operatore * per prodotto  punto double (restituisce un punto)
    public static Point3D operator *(double scalar, Point3D a)
    {
        return new Point3D(
            a.X * scalar,
            a.Y * scalar,
            a.Z * scalar
        );
    }

}

