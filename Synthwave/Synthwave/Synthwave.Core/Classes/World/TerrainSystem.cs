using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Math;

namespace Synthwave.Core.Classes.World;

public class TerrainSystem
{
    public float HeightScale = 18f;
    public float NoiseScale = 0.0025f;

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
}