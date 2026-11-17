# Geometry3D

**Geometry3D** is a C# library exploring the implementation of fundamental three-dimensional geometry and parametric curves. The project follows a bottom-up approach: basic mathematical primitives are introduced first, and more advanced geometric objects and curve representations are built on top of them.

The main goals are to develop a clear and modular implementation of computational geometry algorithms, with particular attention to their mathematical foundations, numerical behaviour, and software testing.

## Project structure


NURBS/
├── NURBS.slnx
├── src/
│   └── Geometry3D/
│       ├── Core/
│       │   ├── Point3D.cs
│       │   ├── Vector3D.cs
│       │   ├── Line3D.cs
│       │   ├── Segment3D.cs
│       │   ├── Plane3D.cs
│       │   └── Circle.cs
│       ├── Curves/
│       │   ├── BezierCurves.cs
│       │   ├── BSplinesCurves.cs
│       │   └── NurbsCurves.cs
│       └── Geometry3D.csproj
├── examples/
│   └── Geometry3D.Demo/
│       ├── Program.cs
│       └── Geometry3D.Demo.csproj
├── tests/
│   └── Geometry3D.Tests/
│       ├── Point3DTests.cs
│       ├── Vector3DTests.cs
│       ├── Line3DTests.cs
│       ├── Segment3DTests.cs
│       ├── Plane3DTests.cs
│       ├── CircleTests.cs
│       ├── BezierCurveTests.cs
│       ├── BSplineCurveTests.cs
│       └── Geometry3D.Tests.csproj
└── ReadMe/
    └── ReadMe.txt


## Core geometry

### Point3D

Represents a point in Cartesian 3D space. It provides point-to-point distance, translation by a vector, linear interpolation, and basic operators. Subtracting two points produces a "Vector3D".

### Vector3D

Represents a free vector in 3D space. The class implements the Euclidean norm, vector addition and subtraction, scalar multiplication, dot product, and cross product. These operations provide the algebraic foundation for projections, distances, and intersection algorithms.

### Line3D and Segment3D

A "Line3D" is represented parametrically as

\[
P(t) = P_0 + t\,\mathbf{d}.
\]

It supports point evaluation, containment tests, closest-point calculations, minimum distance between lines, and line-line intersection detection.

A "Segment3D" is defined by its start and end points. It provides length calculation, parameter evaluation over the interval [0, 1], containment tests, and closest-point projection with parameter clamping.

### Plane3D

A "Plane3D" is defined by an origin point and a normal vector. Its operations include signed distance, containment testing, orthogonal projection, line-plane intersection, and plane-plane intersection. Plane-plane intersection distinguishes parallel, coincident, and intersecting planes.

### Circle

Currently represents a circle through its radius and provides area and circumference calculations.

## Parametric curves

### Bezier curves

"BezierCurve" represents polynomial and rational Bezier curves in 3D. A curve is defined by its control points and, optionally, positive weights.

Evaluation is implemented using the **De Casteljau algorithm** in homogeneous 4D coordinates, which also supports rational curves. Polynomial curves can additionally be evaluated using the Bernstein basis.

The implementation also includes:
- Curve splitting through the De Casteljau algorithm.
- First derivative evaluation, including the quotient rule for rational curves.
- Input validation for control points, weights, and parameter values.

### B-spline curves

"BSplineCurve" represents non-rational B-spline curves defined by control points, degree, and a non-decreasing knot vector.

The implementation includes:
- Input validation for control points, degree, and knot vector.
- Open-uniform knot vector generation over [0, 1].
- Curve evaluation using the **De Boor algorithm**.
- Support for repeated internal knots.

Unlike Bezier curves, B-splines have local support: modifying a control point affects only a limited portion of the curve.

The implementation and its numerical behaviour are still being developed and tested.

### NURBS curves

"NurbsCurve" extends the B-spline representation by associating a positive weight with each control point.

This allows the representation of rational curves and geometries such as circles and circular arcs.

The intended evaluation approach is the De Boor algorithm in homogeneous 4D coordinates, followed by projection back into 3D space.

Further testing and validation of the implementation are still required.

## Automated testing

The solution includes a dedicated **xUnit test project**:

"tests/Geometry3D.Tests"

The tests cover the main geometric primitives, Bezier curves, and B-spline curves.

Current test status:

- Core geometry: 66 tests.
- Bezier curves: 39 tests.
- B-spline curves: 24 tests.
- **Total: 129 passing tests.**

NURBS-specific tests have not yet been implemented. Test coverage is also not exhaustive: additional edge cases, numerical stability checks, and mathematical validation are still needed, particularly for higher-degree curves and non-uniform knot vectors.

Run the complete test suite from the repository root:

"""bash
dotnet test NURBS.slnx
"""

## Demo

The executable project "examples/Geometry3D.Demo" contains usage examples for the implemented classes and algorithms, including distances, vector operations, projections, intersections, closest points, and parametric curve evaluation.

Run the demo from the repository root:

"""bash
dotnet run --project examples/Geometry3D.Demo/Geometry3D.Demo.csproj
"""

## Design principles and future development

The project favours explicit mathematical formulations, readable implementations, and modular code.

Floating-point comparisons are handled with numerical tolerances where appropriate, since exact equality is generally unreliable in computational geometry.

Planned improvements include:

- Further validation of B-spline evaluation, particularly for higher degrees and non-uniform knot vectors.
- Completion and testing of NURBS functionality.
- Derivative evaluation for B-spline and NURBS curves.
- Curve insertion and refinement algorithms.
- Intersection algorithms for parametric curves.
- Additional geometric primitives and surface representations.
- Expanded automated testing, including numerical accuracy and edge cases.

The project is under active development. Its current implementation provides a foundation for exploring computational geometry, numerical algorithms, and parametric curve representations in C#.