namespace Geometry3D.Core;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("=== Test per le classi geometriche ===");
        Console.WriteLine("Lo scopo di questo programma è mostrare casi semplici, dove il risultato è facile da prevedere, e poi alcuni esempi più interessanti.\n");

        RunPoint3DExamples();
        RunLineAndPlaneExamples();
        RunBezierExamples();
        RunBSplineExamples();
        RunNurbsExamples();
    }

    private static void RunPoint3DExamples()
    {
        Console.WriteLine("--- 1) Esempio base con Point3D ---");
        var p0 = new Point3D(0, 0, 0);
        var p1 = new Point3D(2, 0, 0);

        Console.WriteLine("Abbiamo due punti semplici: p0=(0,0,0) e p1=(2,0,0).");
        Console.WriteLine($"Distanza tra i due punti: {p0.DistanceTo(p1)}");
        Console.WriteLine("Se il metodo è giusto, questo valore deve essere 2.\n");
    }

    private static void RunLineAndPlaneExamples()
    {
        Console.WriteLine("--- 2) Line3D e Plane3D: intersezioni e proiezioni ---");

        var pointOnLine = new Point3D(0, 0, 0);
        var direction = new Vector3D(1, 1, 0);
        var line = new Line3D(pointOnLine, direction);

        Console.WriteLine("Costruiamo una retta passante per (0,0,0) con direzione (1,1,0).");
        Console.WriteLine("Poi prendiamo un piano XY e testiamo l'intersezione con la retta.");

        var plane = new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1));
        var hasIntersection = plane.TryIntersectLinePlane(line, out var intersectionPoint);

        Console.WriteLine($"La retta interseca il piano? {hasIntersection}");
        if (hasIntersection)
        {
            PrintPoint("Punto di intersezione", intersectionPoint);
        }

        var pointAbovePlane = new Point3D(1, 2, 3);
        var projectedPoint = plane.Project(pointAbovePlane);
        Console.WriteLine($"Proiezione del punto ({pointAbovePlane.X}, {pointAbovePlane.Y}, {pointAbovePlane.Z}) sul piano: ");
        PrintPoint("Punto proiettato", projectedPoint);

        Console.WriteLine("Testiamo tre casi differenti di intersezione tra piani.");

        var coincidentPlaneA = new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1));
        var coincidentPlaneB = new Plane3D(new Point3D(1, 2, 0), new Vector3D(0, 0, 1));
        var coincidentResult = coincidentPlaneA.TryIntersectPlanePlane(coincidentPlaneB, out var coincidentLine, out var coincidentPlane);
        Console.WriteLine("Caso 1: piani coincidenti");
        Console.WriteLine($"Risultato: {coincidentResult}");
        if (coincidentResult == Plane3D.PlaneIntersectionType.Coincident)
        {
            Console.WriteLine("I piani sono coincidenti, quindi non formano una retta di intersezione unica.");
        }

        var parallelPlaneA = new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1));
        var parallelPlaneB = new Plane3D(new Point3D(0, 0, 2), new Vector3D(0, 0, 1));
        var parallelResult = parallelPlaneA.TryIntersectPlanePlane(parallelPlaneB, out var parallelLine, out var parallelPlane);
        Console.WriteLine("Caso 2: piani paralleli");
        Console.WriteLine($"Risultato: {parallelResult}");
        if (parallelResult == Plane3D.PlaneIntersectionType.Parallel)
        {
            Console.WriteLine("I piani sono paralleli e distinti, quindi non si intersecano.");
        }

        var intersectingPlaneA = new Plane3D(new Point3D(0, 0, 0), new Vector3D(0, 0, 1));
        var intersectingPlaneB = new Plane3D(new Point3D(0, 0, 0), new Vector3D(1, 0, 0));
        var intersectingResult = intersectingPlaneA.TryIntersectPlanePlane(intersectingPlaneB, out var intersectionLine, out var intersectionPlane);
        Console.WriteLine("Caso 3: piani che si intersecano in una retta");
        Console.WriteLine($"Risultato: {intersectingResult}");
        if (intersectingResult == Plane3D.PlaneIntersectionType.Line)
        {
            PrintPoint("Punto sulla retta di intersezione", intersectionLine.Origin);
            Console.WriteLine($"Direzione della retta di intersezione: ({intersectionLine.Direction.X}, {intersectionLine.Direction.Y}, {intersectionLine.Direction.Z})");
        }

        var line1 = new Line3D(new Point3D(0, 0, 0), new Vector3D(1, 0, 0));
        var line2 = new Line3D(new Point3D(0, 1, 1), new Vector3D(0, 1, 0));
        var hasClosestPoints = line1.TryClosestPoints(line2, out var pointOnLine1, out var pointOnLine2, out var distance);

        Console.WriteLine("Testiamo anche la distanza minima tra due rette sghembe.");
        Console.WriteLine($"Esistono punti più vicini? {hasClosestPoints}");
        if (hasClosestPoints)
        {
            PrintPoint("Punto sulla prima retta", pointOnLine1);
            PrintPoint("Punto sulla seconda retta", pointOnLine2);
            Console.WriteLine($"Distanza minima: {distance}");
        }

        Console.WriteLine();
    }

    private static void RunBezierExamples()
    {
        Console.WriteLine("--- 2) Bezier: caso semplice (lineare) ---");
        var linearBezier = new BezierCurve(
            new[] { new Point3D(0, 0, 0), new Point3D(2, 0, 0) },
            new[] { 1.0, 1.0 });

        Console.WriteLine("Con due punti di controllo, la curva di Bezier è semplicemente una retta.");
        PrintPoint("P(0.0)", linearBezier.Evaluate(0.0));
        PrintPoint("P(0.5)", linearBezier.Evaluate(0.5));
        PrintPoint("P(1.0)", linearBezier.Evaluate(1.0));
        Console.WriteLine("In questo caso il punto a t=0.5 dovrebbe essere il punto medio tra i due punti di controllo.\n");

        Console.WriteLine("--- 3) Bezier: caso meno banale (quadratica) ---");
        var quadraticBezier = new BezierCurve(
            new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 2, 0),
                new Point3D(2, 0, 0)
            },
            new[] { 1.0, 1.0, 1.0 });

        double t = 0.5;
        var pointViaEvaluate = quadraticBezier.Evaluate(t);
        var pointViaBernstein = quadraticBezier.EvaluateBernstein(t);
        var derivative = quadraticBezier.Derivative(t);

        Console.WriteLine("Stiamo valutando una Bezier quadratica con tre punti di controllo.");
        PrintPoint($"Evaluate(t={t})", pointViaEvaluate);
        PrintPoint($"EvaluateBernstein(t={t})", pointViaBernstein);
        Console.WriteLine($"Derivative(t={t}): ({derivative.X}, {derivative.Y}, {derivative.Z})");
        Console.WriteLine("Se tutto funziona, Evaluate e EvaluateBernstein devono dare lo stesso risultato entro l'errore numerico.\n");

        Console.WriteLine("--- 4) Bezier: split e punto di giunzione ---");
        var (leftBezier, rightBezier) = quadraticBezier.Split(0.5);
        var splitPointFromLeft = leftBezier.Evaluate(1.0);
        var splitPointFromRight = rightBezier.Evaluate(0.0);
        var originalPoint = quadraticBezier.Evaluate(0.5);

        Console.WriteLine("Quando splitto la curva in due parti, il punto di giunzione deve essere lo stesso se lo valuto da entrambe le sotto-curve.");
        PrintPoint("Punto originale a t=0.5", originalPoint);
        PrintPoint("Punto dalla sotto-curva sinistra (t=1.0)", splitPointFromLeft);
        PrintPoint("Punto dalla sotto-curva destra (t=0.0)", splitPointFromRight);
        Console.WriteLine("Qui vediamo che la curva è stata divisa in due curve distinte, quindi il valore di split non va interpretato come la stessa valutazione della curva originale.\n");
    }

    private static void RunBSplineExamples()
    {
        Console.WriteLine("--- 5) BSpline: caso semplice (lineare) ---");
        var linearBspline = new BSplineCurve(
            new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(2, 0, 0)
            },
            degree: 1,
            knots: new[] { 0.0, 0.0, 1.0, 1.0 });

        Console.WriteLine("Con grado 1 e solo due punti di controllo, la BSpline diventa una retta segmentata.");
        PrintPoint("P(0.0)", linearBspline.Evaluate(0.0));
        PrintPoint("P(0.5)", linearBspline.Evaluate(0.5));
        PrintPoint("P(1.0)", linearBspline.Evaluate(1.0));
        Console.WriteLine("Per questo esempio semplice, il punto a t=0.5 deve stare a metà tra i due punti di controllo.\n");

        Console.WriteLine("--- 6) BSpline: caso meno banale ---");
        var controlPoints = new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 3, 0),
            new Point3D(3, 3, 0),
            new Point3D(4, 0, 0),
            new Point3D(5, 2, 0)
        };

        int degree = 3;
        double[] knots = BSplineCurve.CreateOpenUniformKnots(controlPoints.Length, degree);
        var bspline = new BSplineCurve(controlPoints, degree, knots);

        Console.WriteLine("Questa è una BSpline di grado 3 costruita con nodi open-uniform.");
        PrintPoint("P(0.35)", bspline.Evaluate(0.35));
        Console.WriteLine("Il risultato è più interessante perché la curva è influenzata dal numero di nodi e dal grado.\n");
    }

    private static void RunNurbsExamples()
    {
        Console.WriteLine("--- 7) NURBS: caso semplice con pesi tutti uguali ---");
        var nurbsControlPoints = new[]
        {
            new Point3D(0, 0, 0),
            new Point3D(1, 2, 0),
            new Point3D(2, 0, 0)
        };

        var uniformWeights = new[] { 1.0, 1.0, 1.0 };
        var uniformKnots = new[] { 0.0, 0.0, 0.0, 1.0, 1.0, 1.0 };
        var nurbsUniform = new NurbsCurve(nurbsControlPoints, uniformWeights, degree: 2, uniformKnots);

        Console.WriteLine("Se tutti i pesi sono 1, una NURBS diventa una BSpline non razionale.");
        PrintPoint("NURBS uniform weights at t=0.5", nurbsUniform.Evaluate(0.5));
        Console.WriteLine("Il valore atteso è quindi quello di una BSpline con gli stessi control points e nodi.\n");

        Console.WriteLine("--- 8) NURBS: caso meno banale con pesi diversi ---");
        var weightedNurbs = new NurbsCurve(
            new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 2, 0),
                new Point3D(3, 2, 0),
                new Point3D(4, 0, 0)
            },
            new[] { 1.0, 0.5, 0.5, 1.0 },
            degree: 2,
            knots: new[] { 0.0, 0.0, 0.0, 0.5, 1.0, 1.0, 1.0 });

        Console.WriteLine("Qui usiamo pesi diversi per vedere l'effetto della razionalità.");
        PrintPoint("NURBS weighted at t=0.35", weightedNurbs.Evaluate(0.35));
        var weightedDerivative = weightedNurbs.Derivative(0.35);
        Console.WriteLine($"Derivative at t=0.35: ({weightedDerivative.X}, {weightedDerivative.Y}, {weightedDerivative.Z})");
        Console.WriteLine("Campionamento della curva: ");
        foreach (var sample in weightedNurbs.Sample(5))
        {
            PrintPoint("sample", sample);
        }
    }

    private static void PrintPoint(string label, Point3D point)
    {
        Console.WriteLine($"{label}: ({point.X}, {point.Y}, {point.Z})");
    }
}
