using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Math;
using System;

namespace Synthwave.Core.Classes.World.Biomes;

public static class BiomeSystem
{
    #region Methods
    public static float GetSeaInfluence(float x, float z)
    {
        float h = Noise.Perlin(x * 0.001f, z * 0.001f);
        return MathHelper.Clamp(1f - h, 0f, 1f);
    }

    public static float GetGrassInfluence(float x, float z)
    {
        float h = Noise.Perlin(x * 0.0012f, z * 0.0012f);
        return MathHelper.Clamp(h, 0f, 1f);
    }

    public static float GetSandInfluence(float x, float z)
    {
        float h = Noise.Perlin(x * 0.0018f, z * 0.0018f);
        return MathHelper.Clamp(Math.Abs(h - 0.5f) * 2f, 0f, 1f);
    }

    public static Color GetBiomeColor(float x, float z)
    {
        float sea = GetSeaInfluence(x, z);
        float grass = GetGrassInfluence(x, z);
        float sand = GetSandInfluence(x, z);

        Vector3 seaC = new(0.1f, 0.3f, 0.9f);
        Vector3 grassC = new(0.2f, 1f, 0.2f);
        Vector3 sandC = new(1f, 0.9f, 0.6f);
        Vector3 defC = new(1f, 0.2f, 0.8f);

        Vector3 col =
            seaC * sea +
            grassC * grass +
            sandC * sand +
            defC * (1f - (sea + grass + sand));

        return new Color(col);
    }
    #endregion
}
