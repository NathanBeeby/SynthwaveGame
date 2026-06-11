using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Enums;

namespace Synthwave.Core.Classes.World.Biomes;

public static class BiomeColors
{
    public static Vector3 Get(BiomeType biome)
    {
        return biome switch
        {
            BiomeType.Sea => new Vector3(0.05f, 0.2f, 0.6f),
            BiomeType.Grass => new Vector3(0.2f, 1.0f, 0.2f),
            BiomeType.Sand => new Vector3(1.0f, 0.95f, 0.6f),
            _ => new Vector3(1.0f, 0.1f, 0.8f) // neon pink default
        };
    }
}