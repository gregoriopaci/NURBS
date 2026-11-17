namespace Geometry3D.Core;

public class NurbsCurve
{
    // Properties
    public IReadOnlyList<Point3D> ControlPoints { get; }
    public IReadOnlyList<double> Weights { get; }
    public IReadOnlyList<double> Knots { get; }
    public int Degree { get; }

    // Il dominio parte dal nodo Knots[Degree] e termina al nodo Knots[ControlPoints.Count].
    public double DomainStart
    {
        get
        {
            return Knots[Degree];
        }
    }

    public double DomainEnd
    {
        get
        {
            return Knots[ControlPoints.Count];
        }
    }

    // Costruttore della curva NURBS.
    // Gli argomenti devono essere validi, altrimenti viene sollevata un’eccezione.
    public NurbsCurve(
        IEnumerable<Point3D> controlPoints,
        IEnumerable<double> weights,
        int degree,
        IEnumerable<double> knots)
    {
        ArgumentNullException.ThrowIfNull(controlPoints);
        ArgumentNullException.ThrowIfNull(weights);
        ArgumentNullException.ThrowIfNull(knots);

        ControlPoints = controlPoints.ToList();
        Weights = weights.ToList();
        Knots = knots.ToList();
        Degree = degree;

        if (ControlPoints.Count < 2)
            throw new ArgumentException("A NURBS curve requires at least two control points.", nameof(controlPoints));

        if (Weights.Count != ControlPoints.Count)
            throw new ArgumentException("The number of weights must match the number of control points.", nameof(weights));

        if (degree < 1 || degree >= ControlPoints.Count)
            throw new ArgumentOutOfRangeException(nameof(degree), "The degree must be between 1 and ControlPoints.Count - 1.");

        if (Weights.Any(w => w <= 0))
            throw new ArgumentException("Weights must be positive.", nameof(weights));

        int expectedKnotCount = ControlPoints.Count + degree + 1;
        if (Knots.Count != expectedKnotCount)
            throw new ArgumentException($"Expected {expectedKnotCount} knots for {ControlPoints.Count} control points and degree {degree}.", nameof(knots));

        for (int i = 0; i < Knots.Count; i++)
        {
            if (!double.IsFinite(Knots[i]))
                throw new ArgumentException("Knots must be finite values.", nameof(knots));

            if (i > 0 && Knots[i] < Knots[i - 1])
                throw new ArgumentException("Knots must be non-decreasing.", nameof(knots));
        }

        if (DomainStart >= DomainEnd)
            throw new ArgumentException("The curve domain must have positive width.", nameof(knots));
    }
    // Trova l’intervallo di nodi per un parametro t dato.
    private int FindSpan(double t)
    {
        int lastControlPointIndex = ControlPoints.Count - 1;

        if (t == DomainEnd)
            return lastControlPointIndex;

        for (int i = Degree; i <= lastControlPointIndex; i++)
        {
            if (Knots[i] <= t && t < Knots[i + 1])
                return i;
        }

        throw new InvalidOperationException("Unable to find the knot span for the requested parameter.");
    }

    // Una struct read only per rappresentare un punto in coordinate omogenee.
    // Responsabilità: convertire tra coordinate omogenee e 3D, interpolare tra due punti omogenei.
    // record mi permette di non implementare properties e costruttori manualmente. 
    // Inoltre in questo modo possono essere confrontati con l’operatore ==, che dà true se tutti i campi sono uguali.
    private readonly record struct HomogeneousPoint(double X, double Y, double Z, double W)
    {
        public static HomogeneousPoint From(Point3D p, double w)
        {
            return new HomogeneousPoint(p.X * w, p.Y * w, p.Z * w, w);
        }

        public Point3D ProjectTo3D()
        {
            if (W == 0)
                throw new InvalidOperationException("Point at infinity.");

            return new Point3D(X / W, Y / W, Z / W);
        }

        public static HomogeneousPoint Lerp(HomogeneousPoint a, HomogeneousPoint b, double t)
        {
            return new HomogeneousPoint(
                a.X * (1 - t) + b.X * t,
                a.Y * (1 - t) + b.Y * t,
                a.Z * (1 - t) + b.Z * t,
                a.W * (1 - t) + b.W * t);
        }

        public static HomogeneousPoint operator +(HomogeneousPoint a, HomogeneousPoint b)
        {
            return new HomogeneousPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static HomogeneousPoint operator *(double scalar, HomogeneousPoint p)
        {
            return new HomogeneousPoint(scalar * p.X, scalar * p.Y, scalar * p.Z, scalar * p.W);
        }

        public static HomogeneousPoint operator *(HomogeneousPoint p, double scalar)
        {
            return scalar * p;
        }
    }

    // Valutazione della curva NURBS con l’algoritmo di De Boor e usando coordinate omogenee.
    // Il vantaggio è che nello spazio omogeneo la curva NURBS diventa una curva B-spline non razionale, 
    // quindi possiamo usare lo stesso algoritmo di valutazione usato per le B-spline e poi proiettare il risultato nello spazio 3D.
    public Point3D Evaluate(double t)
    {
        if (t < DomainStart || t > DomainEnd)
            throw new ArgumentOutOfRangeException(nameof(t), $"The parameter must be in [{DomainStart}, {DomainEnd}].");

        // lo span è il valore i tale che Knots[i] <= t < Knots[i + 1]
        int span = FindSpan(t);

        // Copia dei p + 1 punti di controllo coinvolti nello span e conversione in coordinate omogenee.
        // Nell'intervallo [Knots[span], Knots[span + 1]) solo i punti di controllo
        // da ControlPoints[span - Degree] a ControlPoints[span] sono coinvolti.
        var d = new HomogeneousPoint[Degree + 1];
        for (int j = 0; j <= Degree; j++)
        {
            int index = span - Degree + j;
            d[j] = HomogeneousPoint.From(ControlPoints[index], Weights[index]);
        }

        // 
        for (int r = 1; r <= Degree; r++)
        {
            for (int j = Degree; j >= r; j--)
            {
                int knotIndex = span - Degree + j;
                double denominator = Knots[knotIndex + Degree - r + 1] - Knots[knotIndex];
                // Sintassi: condizione ? valoreSeVero : valoreSeFalso;
                double alpha = denominator == 0.0 ? 0.0 : (t - Knots[knotIndex]) / denominator;

                d[j] = HomogeneousPoint.Lerp(d[j - 1], d[j], alpha);
            }
        }

        return d[Degree].ProjectTo3D();
    }

    // Calcola e restituisce count punti sulla curva distribuiti uniformemente nel suo dominio parametrico.
    public IReadOnlyList<Point3D> Sample(int count)
    {
        if (count < 2)
            throw new ArgumentOutOfRangeException(
                nameof(count), "At least two sample points are required.");

        var points = new Point3D[count];

        // Distribuzione uniforme dei parametri nel dominio della curva. 
        // (double) i / (count - 1) (nota il cast) genera valori da 0 a 1, che poi vengono scalati al dominio della curva.
        for (int i = 0; i < count; i++)
        {
            double s = (double)i / (count - 1);
            double t = DomainStart + s * (DomainEnd - DomainStart);
            points[i] = Evaluate(t);
        }

        return points;
    }

    // Calcola la derivata della curva NURBS in un punto t.
    // L'idea è quella di derivare la curva nello spazio omogeneo e poi proiettare il risultato
    // nello spazio 3D tramite la regola del quoziente per curve razionali: R' = (N' w - N w') / w^2..
    public Vector3D Derivative(double t)
    {
        if (t < DomainStart || t > DomainEnd)
            throw new ArgumentOutOfRangeException(nameof(t), $"The parameter must be in [{DomainStart}, {DomainEnd}].");

        if (Degree < 1)
            throw new InvalidOperationException("A NURBS curve must have degree at least 1 to have a derivative.");

        // Trovo l'intervallo t_i <= t < t_{i+1} in cui si trova il parametro t. Questo è necessario per calcolare le funzioni di base e le loro derivate.
        int span = FindSpan(t);

        // Inizializzo i punti omogenei della curva e le loro derivate da riempire con la somma pesata dei punti di controllo e delle loro derivate.
        var curvePoint = new HomogeneousPoint(0, 0, 0, 0);
        var curveDerivative = new HomogeneousPoint(0, 0, 0, 0);

        for (int j = 0; j <= Degree; j++)
        {
            int index = span - Degree + j;
            var homogeneousPoint = HomogeneousPoint.From(ControlPoints[index], Weights[index]);
            double basisValue = BasisFunctionValue(index, Degree, t);
            double basisDerivative = BasisFunctionDerivative(index, Degree, t);

            curvePoint += basisValue * homogeneousPoint;
            curveDerivative += basisDerivative * homogeneousPoint;
        }

        double w = curvePoint.W;
        double wPrime = curveDerivative.W;

        return new Vector3D(
            (curveDerivative.X * w - curvePoint.X * wPrime) / (w * w),
            (curveDerivative.Y * w - curvePoint.Y * wPrime) / (w * w),
            (curveDerivative.Z * w - curvePoint.Z * wPrime) / (w * w));
    }

    // Calcola il valore di una funzione di base B-spline di grado specificato nel punto t.
    private double BasisFunctionValue(int index, int degree, double t)
    {
        if (index < 0 || index >= Knots.Count - 1)
            return 0.0;

        if (degree == 0)
        {
            if (Knots[index] <= t && t < Knots[index + 1])
                return 1.0;

            if (Math.Abs(t - DomainEnd) < 1e-12 && index == ControlPoints.Count - 1)
                return 1.0;

            return 0.0;
        }

        double left = 0.0;
        double right = 0.0;

        double leftDenominator = Knots[index + degree] - Knots[index];
        if (Math.Abs(leftDenominator) > 1e-12)
        {
            left = (t - Knots[index]) / leftDenominator * BasisFunctionValue(index, degree - 1, t);
        }

        double rightDenominator = Knots[index + degree + 1] - Knots[index + 1];
        if (Math.Abs(rightDenominator) > 1e-12)
        {
            right = (Knots[index + degree + 1] - t) / rightDenominator * BasisFunctionValue(index + 1, degree - 1, t);
        }

        return left + right;
    }

    // Calcola la derivata di una funzione di base B-spline di grado specificato nel punto t.
    private double BasisFunctionDerivative(int index, int degree, double t)
    {
        if (degree < 1)
            return 0.0;

        double leftDenominator = Knots[index + degree] - Knots[index];
        double rightDenominator = Knots[index + degree + 1] - Knots[index + 1];

        double left = 0.0;
        double right = 0.0;

        if (Math.Abs(leftDenominator) > 1e-12)
        {
            left = degree * BasisFunctionValue(index, degree - 1, t) / leftDenominator;
        }

        if (Math.Abs(rightDenominator) > 1e-12)
        {
            right = degree * BasisFunctionValue(index + 1, degree - 1, t) / rightDenominator;
        }

        return left - right;
    }

}
