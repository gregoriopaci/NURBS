
												===== Geometry3D =====



Geometry3D is a small C# project that explores the implementation of fundamental three-dimensional geometry and parametric curves. The code is intentionally organised bottom-up: simple mathematical primitives are introduced first, and higher-level geometric objects are built on top of them.

--- Project structure ---

Geometry3D/ |-- Geometry3D.csproj |-- Core/ |   |-- Point3D.cs |   |-- Vector3D.cs |   |-- Line3D.cs |   |-- Segment3D.cs |   |-- Plane3D.cs |   |-- Circle.cs |-- Curves/ |   |-- BezierCurves.cs |   |-- BSplinesCurves.cs |   |—- NurbsCurves.cs — Demo/     |—- Program.cs



--- Core geometry ---

- Point3D
Represents a point in Cartesian 3D space. It provides point-to-point distance, translation by a vector, linear interpolation, and the basic operators needed by the rest of the library. Subtracting two points produces a Vector3D.

- Vector3D
Represents a free vector in 3D space. The class implements the Euclidean norm, vector addition and subtraction, scalar multiplication, dot product, and cross product. These operations are the algebraic foundation for projections, distances, and intersection tests.

- Line3D and Segment3D
A Line3D is represented parametrically as P(t) = Origin + t * Direction. It can evaluate a point at a parameter, test whether a point belongs to the line, find the closest point to an external point, compute the minimum distance between two lines, and detect a unique line-line intersection.

A Segment3D is defined by its start and end points. In addition to its length, it supports parameter evaluation on the interval [0, 1], point containment, and closest-point projection with parameter clamping.

- Plane3D
A Plane3D is defined by an origin point and a normal vector. The implementation provides signed distance, containment testing, orthogonal projection, line-plane intersection, and plane-plane intersection. Plane-plane intersections distinguish parallel, coincident, and intersecting planes.

- Circle
Circle currently represents a circle through its radius and exposes methods for area and circumference.



--- Curves ---

- Bezier curves
BezierCurve represents polynomial and rational Bezier curves in 3D. A curve is built from its control points and, optionally, positive weights. When all weights are one, the curve is polynomial; otherwise it is rational.

Evaluation is implemented with the De Casteljau algorithm in homogeneous 4D coordinates, which also handles rational curves. Polynomial curves can additionally be evaluated with the Bernstein basis, making it possible to compare the two formulations. The class also provides curve splitting through the De Casteljau triangle and evaluation of the first derivative, including the quotient rule required for rational curves.

- B-spline curves
BSplineCurve represents a non-rational B-spline defined by its control points, degree, and non-decreasing knot vector. It validates the input data and can generate an open-uniform knot vector on the domain [0, 1].

The curve is evaluated with the De Boor algorithm. Unlike a Bezier curve, a B-spline has local support: changing one control point affects only the nearby part of the curve.

- NURBS curves
NurbsCurve extends the B-spline idea by associating a positive weight with each control point. The weights make it possible to represent rational shapes, including conic sections such as circles and arcs.

Evaluation uses the De Boor algorithm in homogeneous 4D coordinates and then projects the result back into 3D space. The class also provides Sample, which returns points uniformly distributed over the curve parameter domain.



--- Demo ---

Demo and execution
Demo/Program.cs contains executable examples that exercise the implemented methods and classes: distances, vector operations, projections, intersections, closest points, and Bezier/B-splines/NURBS evaluation.

From within the repository Geometry3D/ run the demo with:
dotnet run

Design notes
The project favours explicit geometric formulas and readable implementations. Operations involving floating-point comparisons use a tolerance because exact equality is generally not reliable in numerical geometry. The demo is intended as a usage example; a dedicated automated test project is a natural next improvement. Other natural improvement is the introduction of other methods such as the intersection of NURBS curves and 
