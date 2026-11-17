namespace Geometry3D.Core;

public class BezierCurve
{
    // Proprietà per ottenere i punti di controllo della curva di Bezier
    public IReadOnlyList<Point3D> ControlPoints { get; }

    // Vettore di pesi per le Bezier razionali (se tutti 1 => non razionale)
    public IReadOnlyList<double> Weights { get; }

    // Proprietà per ottenere il grado della curva di Bezier
    public int Degree
    {
        get
        {
            return ControlPoints.Count - 1;
        }
    }

    // Costruttore che accetta una lista di punti di controllo e di pesi. 
    // Se i pesi non sono forniti, vengono impostati a 1 (curva non razionale).
    public BezierCurve(
        IEnumerable<Point3D> controlPoints,
        IEnumerable<double>? weights = null)
    {   
        ArgumentNullException.ThrowIfNull(controlPoints);

        var points = controlPoints.ToList();

        if (points.Count < 2)
            throw new ArgumentException(
                "A Bezier curve requires at least two control points.",
                nameof(controlPoints));

        if (points.Any(point => point is null))
            throw new ArgumentException(
                "Control points cannot contain null values.",
                nameof(controlPoints));

        var wList = (weights ??
            Enumerable.Repeat(1.0, points.Count)).ToList();

        if (wList.Count != points.Count)
            throw new ArgumentException(
                "Weights length must match control points length.",
                nameof(weights));

        if (wList.Any(w => !double.IsFinite(w) || w <= 0))
            throw new ArgumentException(
                "Weights must be finite and positive.",
                nameof(weights));

        ControlPoints = points;
        Weights = wList;
    }



    public bool IsRational
    {
        get
        {
            // lambda expression
            return Weights.Any(w => Math.Abs(w - 1.0) > 1e-12);
        }
    }

    // Piccola struttura helper per punti omogenei 4D (x*w, y*w, z*w, w)
    private readonly record struct HomogeneousPoint(double X, double Y, double Z, double W)
    {
        public static HomogeneousPoint From(Point3D p, double w)
        {
            return new HomogeneousPoint(p.X * w, p.Y * w, p.Z * w, w);
        }


        public Point3D ProjectTo3D()
        {
            if (W == 0)
                throw new InvalidOperationException(
            "Point at infinity.");

            return new Point3D(X / W, Y / W, Z / W);
        }

        public static HomogeneousPoint operator *(HomogeneousPoint p, double s)
        {
            return new HomogeneousPoint(s * p.X, s * p.Y, s * p.Z, s * p.W);
        }

        public static HomogeneousPoint operator +(HomogeneousPoint a, HomogeneousPoint b)
        {
            return new HomogeneousPoint(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
        }

        public static HomogeneousPoint operator -(HomogeneousPoint a, HomogeneousPoint b)
        {
            return new HomogeneousPoint(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
        }

        public static HomogeneousPoint Lerp(HomogeneousPoint a, HomogeneousPoint b, double t)
        {
            return a * (1 - t) + b * t;
        }
    }



    // Implemento l'algoritmo di DeCasteljau. Voglio che ritorni una lista di liste di punti, dove ogni lista rappresenta un livello del triangolo di De Casteljau.
    /* private List<List<Point3D>> DeCasteljau(double t)
     {
         var triangle = new List<List<Point3D>>();

         triangle.Add(ControlPoints.ToList());

         while (triangle.Last().Count > 1)
         {
             var previous = triangle.Last();
             var next = new List<Point3D>();

             for (int i = 0; i < previous.Count - 1; i++)
             {
                 next.Add(Point3D.Lerp(previous[i], previous[i + 1], t));
             }

             triangle.Add(next);
         }

         return triangle;
     }*/



    // De Casteljau che opera in spazio omogeneo 4D usando i pesi
    private List<List<HomogeneousPoint>> DeCasteljauHomogeneous(double t)
    {
        var triangle = new List<List<HomogeneousPoint>>();

        // Creo il primo livello del triangolo di De Casteljau in coordinate omogenee
        var first = new List<HomogeneousPoint>();
        // Riempimento del primo livello con i punti di controllo omogenei
        for (int i = 0; i < ControlPoints.Count; i++)
            first.Add(HomogeneousPoint.From(ControlPoints[i], Weights[i]));

        triangle.Add(first);

        while (triangle.Last().Count > 1)
        {
            var previous = triangle.Last();
            var next = new List<HomogeneousPoint>();

            for (int i = 0; i < previous.Count - 1; i++)
            {
                next.Add(previous[i] * (1 - t) + previous[i + 1] * t);
            }

            triangle.Add(next);
        }

        return triangle;
    }



    // Metodo per calcolare un punto su una curva di Bezier nonrazionale usando l'algoritmo di DeCasteljau
    /*public Point3D Evaluate(double t)
    {
        if (t < 0 || t > 1)
            throw new ArgumentOutOfRangeException("t", "Il parametro deve essere compreso tra 0 e 1.");


        if (IsRational)
            return EvaluateRational(t);


        return DeCasteljau(t).Last()[0];
    }*/

    // Valuta la curva come Bezier razionale usando De Casteljau in spazio omogeneo. 
    public Point3D Evaluate(double t)
    {
        if (!double.IsFinite(t) || t < 0 || t > 1)
    throw new ArgumentOutOfRangeException(
        nameof(t),
        "Il parametro deve essere finito e compreso tra 0 e 1.");

        var triangle = DeCasteljauHomogeneous(t);
        return triangle.Last()[0].ProjectTo3D();
    }



    // Metodo per calcolare il coefficiente binomiale "n choose k"
    private double BinomialCoefficient(int n, int k)
    {
        if (k < 0 || k > n)
            throw new ArgumentOutOfRangeException("k", "Il parametro k deve essere compreso tra 0 e n.");

        if (k == 0 || k == n)
            return 1;

        double result = 1;
        for (int i = 1; i <= k; i++)
        {
            result *= (n - (k - i));
            result /= i;
        }
        return result;
    }

    // Metodo per calcolare un punto su una curva di Bezier usando la formula polinomiale
    public Point3D EvaluateBernstein(double t)
    {
        if (!double.IsFinite(t) || t < 0 || t > 1)
    throw new ArgumentOutOfRangeException(
        nameof(t),
        "Il parametro deve essere finito e compreso tra 0 e 1.");

        if (IsRational)
            throw new InvalidOperationException(
            "Bernstein evaluation applies only to polynomial Bezier curves.");

        Point3D point = new Point3D(0, 0, 0);
        int n = Degree;

        for (int i = 0; i <= n; i++)
        {
            double binomialCoefficient = BinomialCoefficient(n, i);
            double basis = binomialCoefficient * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
            point += basis * ControlPoints[i];
        }

        return point;
    }


    // Split la curva in due sub-curves usando DeCasteljauHomogeneous. Restituisce due nuove curve di Bezier, 
    // una per la parte sinistra e una per la parte destra della curva originale.
    public (BezierCurve Left, BezierCurve Right) Split(double t)
    {
        if (!double.IsFinite(t) || t < 0 || t > 1)
        throw new ArgumentOutOfRangeException(
            nameof(t),
            "Il parametro deve essere finito e compreso tra 0 e 1.");


        var triangle = DeCasteljauHomogeneous(t);


        var leftHomogeneous = new List<HomogeneousPoint>();
        var rightHomogeneous = new List<HomogeneousPoint>();

        for (int i = 0; i < triangle.Count; i++)
        {
            leftHomogeneous.Add(triangle[i][0]);
            rightHomogeneous.Add(triangle[i].Last());
        }

        rightHomogeneous.Reverse();

        var leftPoints = leftHomogeneous
            .Select(p => p.ProjectTo3D())
            .ToList();

        var rightPoints = rightHomogeneous
            .Select(p => p.ProjectTo3D())
            .ToList();


        var leftWeights = leftHomogeneous
            .Select(p => p.W)
            .ToList();

        var rightWeights = rightHomogeneous
            .Select(p => p.W)
            .ToList();


        return (
            new BezierCurve(leftPoints, leftWeights),
            new BezierCurve(rightPoints, rightWeights)
        );
    }



    // Calcola il vettore derivata in un punto t.
    // Per curve razionali applica la regola del quoziente: R' = (N' w - N w') / w^2, 
    // dove C(t) = N(t)/w(t) e N(t) = sum_i B_i^n(t) P_i w_i, w(t) = sum_i B_i^n(t) w_i.
    public Vector3D Derivative(double t)
    {
        if (!double.IsFinite(t) || t < 0 || t > 1)
            throw new ArgumentOutOfRangeException(
            nameof(t),
            "Il parametro deve essere finito e compreso tra 0 e 1.");

        if (Degree < 1)
            throw new InvalidOperationException(
                "La curva di Bezier deve avere almeno due punti di controllo per calcolare la derivata.");


        int n = Degree;
        // Triangolo di De Casteljau in coordinate omogenee
        var triangle = DeCasteljauHomogeneous(t);

        var lastDeCasteljauLevel = triangle.Count - 1;

        // Punto omogeneo sulla curva
        HomogeneousPoint h = triangle[lastDeCasteljauLevel][0];

        // Penultimo livello del triangolo
        var penultimate = triangle[lastDeCasteljauLevel - 1];

        // Derivata del punto omogeneo. Uso che la derivata di una Bezier di grado n è n volte la differenza tra i 2 punti del penultimo livello.
        // C'(t) = n * (P_{n-1}(t) - P_{n-2}(t))
        HomogeneousPoint dh = (penultimate[1] - penultimate[0]) * n;

        // Proiezione tramite la regola del quoziente
        double w = h.W;
        double wPrime = dh.W;

        return new Vector3D(
            (dh.X * w - h.X * wPrime) / (w * w),
            (dh.Y * w - h.Y * wPrime) / (w * w),
            (dh.Z * w - h.Z * wPrime) / (w * w)
        );
    }





}




// questo usa i polinomi di Bernstein per calcolare la derivata. 
/*public Vector3D EvaluateDerivative(double t)
{
if (t < 0 || t > 1)
    throw new ArgumentOutOfRangeException("t", "Il parametro deve essere compreso tra 0 e 1.");

if (Degree < 1)
    throw new InvalidOperationException("La curva di Bezier deve avere almeno due punti di controllo per calcolare la derivata.");

int n = Degree;

if (!IsRational)
{
    var result = new Vector3D(0, 0, 0);
    for (int i = 0; i < n; i++)
    {
        double basis = BinomialCoefficient(n - 1, i) * Math.Pow(1 - t, n - 1 - i) * Math.Pow(t, i);
        var diff = ControlPoints[i + 1] - ControlPoints[i];
        result += (n * basis) * diff;
    }
    return result;
}

var weightedPointSum = new Vector3D(0, 0, 0);
double weightSum = 0.0;
for (int i = 0; i <= n; i++)
{
    double basis = BinomialCoefficient(n, i) * Math.Pow(1 - t, n - i) * Math.Pow(t, i);
    var p = ControlPoints[i];
    weightedPointSum += (basis * Weights[i]) * new Vector3D(p.X, p.Y, p.Z);
    weightSum += Weights[i] * basis;
}

var weightedPointDerivative = new Vector3D(0, 0, 0);
double weightDerivative = 0.0;
for (int i = 0; i < n; i++)
{
    double basis = BinomialCoefficient(n - 1, i) * Math.Pow(1 - t, n - 1 - i) * Math.Pow(t, i);
    var p0 = ControlPoints[i];
    var p1 = ControlPoints[i + 1];
    double w0 = Weights[i];
    double w1 = Weights[i + 1];

    var diff = new Vector3D(
        w1 * p1.X - w0 * p0.X,
        w1 * p1.Y - w0 * p0.Y,
        w1 * p1.Z - w0 * p0.Z
    );

    weightedPointDerivative += (n * basis) * diff;
    weightDerivative += (n * basis) * (w1 - w0);
}

return ((weightedPointDerivative * weightSum) - (weightedPointSum * weightDerivative)) * (1.0 / (weightSum * weightSum));
}












*/

