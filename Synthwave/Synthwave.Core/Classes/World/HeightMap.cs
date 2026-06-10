using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Terrain;

namespace Synthwave.Core.Classes.World;

public class HeightMap
{
    private readonly Color[] pixels;

    public int Width { get; }
    public int Height { get; }

    public HeightMap(Texture2D texture)
    {
        Width = texture.Width;
        Height = texture.Height;

        pixels = new Color[Width * Height];
        texture.GetData(pixels);
    }

    public float GetHeight(int x, int y)
    {
        x = System.Math.Clamp(x, 0, Width - 1);
        y = System.Math.Clamp(y, 0, Height - 1);

        Color c = pixels[y * Width + x];
        return c.R / 255f;
    }

    public TerrainType GetTerrainType(int x, int y)
    {
        float h = GetHeight(x, y);

        if (h < 0.35f) return TerrainType.Water;
        if (h < 0.40f) return TerrainType.Road;
        return TerrainType.Land;
    }
}