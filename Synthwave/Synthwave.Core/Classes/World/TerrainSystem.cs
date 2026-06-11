using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Math;

namespace Synthwave.Core.Classes.World;

public class TerrainSystem
{
    #region Properties
    public float HeightScale = 18f;
    public float NoiseScale = 0.0025f;
    #endregion

    #region Methods
    public float GetHeight(float x, float z)
    {
        float n1 = Noise.Perlin(x * NoiseScale, z * NoiseScale);
        float n2 = Noise.Perlin(x * NoiseScale * 0.5f, z * NoiseScale * 0.5f);

        float height = (n1 * 0.7f + n2 * 0.3f);

        return height * HeightScale;
    }

    public Vector3 ProjectToTerrain(Vector3 p)
    {
        p.Y = GetHeight(p.X, p.Z);
        return p;
    }

    // -------------------- Biome Sampling --------------------
    public float GetSeaInfluence(float x, float z)
    {
        float h = GetHeight(x, z);
        return MathHelper.Clamp(1f - (h / (HeightScale * 0.3f)), 0f, 1f); // low areas → sea
    }

    public float GetSandInfluence(float x, float z)
    {
        float h = GetHeight(x, z);
        return MathHelper.Clamp((h / HeightScale) * 2f - 0.3f, 0f, 1f); // mid-low areas → sand
    }

    public float GetGrassInfluence(float x, float z)
    {
        float h = GetHeight(x, z);
        return MathHelper.Clamp((h / HeightScale) - 0.5f, 0f, 1f); // higher areas → grass
    }
    #endregion
}