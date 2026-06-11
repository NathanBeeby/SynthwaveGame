using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Structures;

namespace Synthwave.Core.Classes.World.Biomes;

public static class BiomeLibrary
{
    public static readonly BiomeData Default = new() { Color = new Vector3(1.0f, 0.2f, 0.9f) }; // pink
    public static readonly BiomeData Sea = new() { Color = new Vector3(0.1f, 0.3f, 1.0f) }; // dark blue
    public static readonly BiomeData Grass = new() { Color = new Vector3(0.2f, 1.0f, 0.3f) }; // lime
    public static readonly BiomeData Sand = new() { Color = new Vector3(1.0f, 0.95f, 0.6f) }; // light yellow
}
