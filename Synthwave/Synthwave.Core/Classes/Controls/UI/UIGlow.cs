using Microsoft.Xna.Framework;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public static class UIGlow
{
    public static float Pulse(float time, float speed = 3f)
    {
        return 0.6f + 0.4f * MathF.Sin(time * speed);
    }

    public static Color Neon(Color baseColor, float intensity)
    {
        return new Color(
            baseColor.R * intensity,
            baseColor.G * intensity,
            baseColor.B * intensity);
    }
}