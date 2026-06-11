using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.World.Biomes;

public static class BiomeSampler
{
    public static Vector3 GetBiomeColor(float worldX, float worldZ)
    {
        Color col = BiomeSystem.GetBiomeColor(worldX, worldZ);
        return new Vector3(col.R / 255f, col.G / 255f, col.B / 255f);
    }
}