using System;

namespace Synthwave.Core.Classes.Models;

public class NoiseGenerator(float roadWidth)
{
    private readonly float roadWidth = roadWidth;

    public float ModifyHeight(float baseHeight, float x, float z)
    {
        if (MathF.Abs(x) < roadWidth) return baseHeight * 0.2f;
        return baseHeight;
    }
}