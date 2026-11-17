namespace Geometry3D.Core;

public class BSplineCurve
{
    // Properties
    // Punti di controllo della curva B-spline.
    public IReadOnlyList<Point3D> ControlPoints { get; }

    // Vettore dei nodi della curva B-spline. Questa è la novità rispetto alle rationa Bezier curves. 
    // I nodi definiscono il dominio parametrico della curva e la sua forma.
    public IReadOnlyList<double> Knots { get; }

    // Grado della curva B-spline.
    public int Degree { get; }

    // Il dominio parametrico della curva B-spline è definito dai nodi.
    // Il dominio inizia dal nodo Knots[Degree] e termina al nodo Knots[ControlPoints.Count].
    // Questo è un concetto chiave nelle B-spline: il dominio parametrico non è definito dai punti di controllo, ma dai nodi. 
    //
    // Nota: Knots[Degree] è il primo nodo che non è ripetuto Degree volte, e Knots[ControlPoints.Count] è l'ultimo nodo 
    // che non è ripetuto Degree volte. Questo garantisce che la curva sia definita correttamente e abbia il supporto locale desiderato.
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

    // Costruttore della curva B-spline. 
    // Gli argomenti devono essere validi, altrimenti viene sollevata un’eccezione.
    public BSplineCurve(
        IEnumerable<Point3D> controlPoints,
        int degree,
        IEnumerable<double> knots)
    {
        ArgumentNullException.ThrowIfNull(controlPoints);
        ArgumentNullException.ThrowIfNull(knots);

        ControlPoints = controlPoints.ToList();
        Knots = knots.ToList();
        Degree = degree;

        if (ControlPoints.Count < 2)
            throw new ArgumentException(
                "Una B-spline richiede almeno due punti di controllo.",
                nameof(controlPoints));

        if (ControlPoints.Any(point => point is null))
        throw new ArgumentException(
            "I punti di controllo non possono contenere valori null.",
            nameof(controlPoints));
            
        if (degree < 1 || degree >= ControlPoints.Count)
            throw new ArgumentOutOfRangeException(
                nameof(degree),
                "Il grado deve essere compreso tra 1 e ControlPoints.Count - 1.");

        // Con N punti di controllo e grado p:
        // numero nodi = N + p + 1
        int expectedKnotCount = ControlPoints.Count + degree + 1;
        if (Knots.Count != expectedKnotCount)
            throw new ArgumentException(
                $"Per {ControlPoints.Count} punti e grado {degree} servono " +
                $"{expectedKnotCount} nodi.",
                nameof(knots));

        for (int i = 0; i < Knots.Count; i++)
        {
            if (!double.IsFinite(Knots[i]))
                throw new ArgumentException(
                    "I nodi devono essere valori finiti.",
                    nameof(knots));

            if (i > 0 && Knots[i] < Knots[i - 1])
                throw new ArgumentException(
                    "Il vettore dei nodi deve essere non decrescente.",
                    nameof(knots));
        }

        if (DomainStart >= DomainEnd)
            throw new ArgumentException(
                "Il dominio parametrico della curva deve avere ampiezza positiva.",
                nameof(knots));
    }

    // Costruisce nodi open-uniform, con dominio [0, 1].
    public static double[] CreateOpenUniformKnots(
        int controlPointCount,
        int degree)
    {
        if (controlPointCount < 2)
            throw new ArgumentOutOfRangeException(nameof(controlPointCount));

        if (degree < 1 || degree >= controlPointCount)
            throw new ArgumentOutOfRangeException(nameof(degree));

        int knotCount = controlPointCount + degree + 1;
        var knots = new double[knotCount];

        // p + 1 nodi iniziali a zero
        for (int i = 0; i <= degree; i++)
            knots[i] = 0.0;

        // Nodi interni uniformi
        int internalKnotCount = controlPointCount - degree - 1;
        for (int i = 1; i <= internalKnotCount; i++)
            knots[degree + i] = (double)i / (internalKnotCount + 1);

        // p + 1 nodi finali a uno
        for (int i = controlPointCount; i < knotCount; i++)
            knots[i] = 1.0;

        return knots;
    }

    // Trova lo span dei nodi per un parametro t.
    // Lo span è l’indice i tale che Knots[i] <= t < Knots[i + 1].
    // Nel ciclo for si parte da Degree perché i primi Degree nodi sono tutti uguali a Knots[Degree]=0.
    private int FindSpan(double t)
    {
        int lastControlPointIndex = ControlPoints.Count - 1;

        // Il limite destro del dominio appartiene all’ultimo span.
        if (t == DomainEnd)
            return lastControlPointIndex;

        for (int i = Degree; i <= lastControlPointIndex; i++)
        {
            if (Knots[i] <= t && t < Knots[i + 1])
                return i;
        }

        throw new InvalidOperationException(
            "Impossibile trovare lo span dei nodi per il parametro specificato.");
    }

    // Valutazione con algoritmo di De Boor.
    public Point3D Evaluate(double t)
    {
        if (!double.IsFinite(t) || t < DomainStart || t > DomainEnd)
            throw new ArgumentOutOfRangeException(
                nameof(t),
                $"Il parametro deve appartenere a [{DomainStart}, {DomainEnd}].");

        // lo span è il valore i tale che Knots[i] <= t < Knots[i + 1]
        int span = FindSpan(t);

        // Copia dei p + 1 punti di controllo localmente coinvolti:
        // la B-spline ha supporto locale.
        // Nell'intervallo [Knots[span], Knots[span + 1]) solo i punti di controllo
        // da ControlPoints[span - Degree] a ControlPoints[span] sono coinvolti.
        var d = new Point3D[Degree + 1];

        // Copia dei p + 1 punti di controllo coinvolti
        for (int j = 0; j <= Degree; j++)
        {
            Point3D point = ControlPoints[span - Degree + j];
            d[j] = new Point3D(point.X, point.Y, point.Z);
        }

        // Cuore dell’algoritmo di De Boor: 
        // iterazioni per ridurre il numero di punti di controllo fino a uno.
        for (int r = 1; r <= Degree; r++)
        {
            for (int j = Degree; j >= r; j--)
            {
                int knotIndex = span - Degree + j;
                double denominator = Knots[knotIndex + Degree - r + 1]
                                   - Knots[knotIndex];
                // Sintassi: condizione ? valoreSeVero : valoreSeFalso;
                double alpha = denominator == 0.0 ? 0.0 : (t - Knots[knotIndex]) / denominator;

                d[j] = Point3D.Lerp(d[j - 1], d[j], alpha);
            }
        }

        return d[Degree];
    }


}