using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core.Math;

public class Spline
{
    #region Properties
    public List<Vector3> Points = [];
    #endregion

    #region Methods
    public Vector3 Evaluate(float t)
    {
        if (Points.Count == 0) return Vector3.Zero;
        if (Points.Count == 1) return Points[0];

        t = MathHelper.Clamp(t, 0f, 1f);

        int segments = Points.Count - 1;
        float scaled = t * segments;
        int i = (int)scaled;
        if (i >= segments) i = segments - 1;
        float local = scaled - i;

        // Catmull-Rom control points (clamped at ends)
        Vector3 p0 = Points[System.Math.Max(0, i - 1)];
        Vector3 p1 = Points[i];
        Vector3 p2 = Points[System.Math.Min(Points.Count - 1, i + 1)];
        Vector3 p3 = Points[System.Math.Min(Points.Count - 1, i + 2)];

        return CatmullRom(p0, p1, p2, p3, local);
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }
    #endregion
}
