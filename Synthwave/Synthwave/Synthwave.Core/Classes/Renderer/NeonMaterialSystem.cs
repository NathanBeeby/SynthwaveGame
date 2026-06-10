using Microsoft.Xna.Framework;
using System;

namespace Synthwave.Core.Classes.Renderer;

public class NeonMaterialSystem
{
    #region Methods
    public static Vector3 Evaluate(float t, float intensity)
    {
        float r = 0.5f + 0.5f * MathF.Sin(t * MathF.Tau * 2f);
        float g = 0.1f;
        float b = 0.5f + 0.5f * MathF.Cos(t * MathF.Tau * 2f);
        return new Vector3(r, g, b) * intensity;
    }
    #endregion
}
