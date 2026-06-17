using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Math;

namespace Synthwave.Core.Classes.World;

public class TerrainSystem
{
    #region Properties
    // Smaller amplitude ("slight deviations") and higher base frequency
    // ("frequent hills"). Tune to taste — HeightScale ~4-8 and
    // NoiseScale ~0.008-0.015 give gentle, frequent bumps rather than
    // sweeping mountains.
    public float HeightScale = 6f;
    public float NoiseScale = 0.01f;

    private float[,] _heightMap;
    private float[,] _waterMap;

    public int Width { get; private set; }
    public int Length { get; private set; }
    #endregion

    #region Constructor
    public TerrainSystem(int width, int length)
    {
        Width = width;
        Length = length;

        _heightMap = new float[Width, Length];
        _waterMap = new float[Width, Length];

        for (int x = 0; x < Width; x++)
        {
            for (int z = 0; z < Length; z++)
            {
                _heightMap[x, z] = GetHeight(x, z);
                _waterMap[x, z] = 0f;
            }
        }
    }
    #endregion

    #region Methods
    public float GetWaterLevel(float x, float z)
    {
        int ix = (int)MathHelper.Clamp(x, 0, Width - 1);
        int iz = (int)MathHelper.Clamp(z, 0, Length - 1);
        return _waterMap[ix, iz];
    }

    public void SetPuddle(int x, int z, float amount)
    {
        if (x < 0 || x >= Width || z < 0 || z >= Length) return;
        _waterMap[x, z] = MathHelper.Clamp(amount, 0f, 1f);
    }

    public float GetHeight(float x, float z)
    {
        // Base layer: broad gentle undulation.
        float n1 = Noise.Perlin(x * NoiseScale, z * NoiseScale);
        // Second layer at double the frequency adds extra small bumps
        // on top, instead of the old lower-frequency layer that just
        // made the big hills even bigger.
        float n2 = Noise.Perlin(x * NoiseScale * 2f, z * NoiseScale * 2f);
        return (n1 * 0.7f + n2 * 0.3f) * HeightScale;
    }


    public Vector3 ProjectToTerrain(Vector3 p)
    {
        p.Y = GetHeight(p.X, p.Z);
        return p;
    }
    #endregion
}