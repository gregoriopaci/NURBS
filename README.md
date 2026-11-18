# Geometry3D

**Geometry3D** is a C# library for three-dimensional computational geometry and parametric curve representations.

The project focuses on modular implementations of geometric primitives and curve evaluation algorithms, with attention to mathematical foundations, numerical behaviour, and automated testing.

## Project Structure

```text
NURBS/
├── src/Geometry3D/
│   ├── Core/
│   ├── Curves/
│   └── Geometry3D.csproj
├── examples/
│   └── Geometry3D.Demo/
├── tests/
│   └── Geometry3D.Tests/
├── NURBS.slnx
└── README.md
```

## Implemented Features

### Core Geometry

- **Point3D:** Cartesian points, distances, translations, interpolation, and arithmetic operations.
- **Vector3D:** Vector algebra, norms, dot and cross products.
- **Line3D:** Parametric representation, point containment, distances, closest points, and intersections.
- **Segment3D:** Length, parameter evaluation, containment, and closest-point projection.
- **Plane3D:** Signed distances, projections, and line-plane and plane-plane intersections.
- **Circle:** Area and circumference calculations.

### Parametric Curves

- **Bezier curves:** Polynomial and rational evaluation using the De Casteljau algorithm, Bernstein basis, curve splitting, and first derivatives.
- **B-spline curves:** Knot vector generation, validation, and evaluation using the De Boor algorithm, including repeated internal knots.
- **NURBS curves:** Rational B-spline representation with positive control-point weights. Implementation and testing are ongoing.

## Testing

Automated tests are implemented using **xUnit**.

| Component | Tests |
|---|---:|
| Core geometry | 66 |
| Bezier curves | 39 |
| B-spline curves | 24 |
| **Total** | **129 passing** |

NURBS-specific tests are not yet implemented.

Run the test suite:

```bash
dotnet test NURBS.slnx
```

## Demo

The `Geometry3D.Demo` project provides examples of geometric operations, intersections, projections, and parametric curve evaluation.

Run the demo:

```bash
dotnet run --project examples/Geometry3D.Demo/Geometry3D.Demo.csproj
```

## Future Development

Planned improvements include:

- NURBS implementation and validation.
- B-spline and NURBS derivatives.
- Curve insertion and refinement algorithms.
- Parametric curve intersections.
- Additional geometric primitives and surface representations.
- Expanded numerical accuracy and edge-case testing.

The project is under active development and serves as a foundation for exploring computational geometry and numerical methods in C#.