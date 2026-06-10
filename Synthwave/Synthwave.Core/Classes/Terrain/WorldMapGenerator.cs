using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Synthwave.Core.Classes.Terrain;

// TextureImporter
// TextureProcessor

public class WorldMapGenerator
{
    public static void Run()
    {
        int size = 4096;

        using Bitmap bmp = new Bitmap(size, size);

        Random rand = new(1337);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float nx = (float)x / size - 0.5f;
                float ny = (float)y / size - 0.5f;

                float d = Distance(nx, ny);

                float height =
                    Noise(nx * 6f, ny * 6f, rand) * 0.6f +
                    Noise(nx * 12f, ny * 12f, rand) * 0.3f +
                    Noise(nx * 24f, ny * 24f, rand) * 0.1f;

                // Ocean basin
                height -= d * 1.2f;

                // Flatten road corridor (central highway strip)
                if (System.Math.Abs(nx) < 0.03f)
                    height = Lerp(height, 0.55f, 0.7f);

                height = Clamp01(height);

                int v = (int)(height * 255);

                Color c = Color.FromArgb(v, v, v);

                bmp.SetPixel(x, y, c);
            }
        }

        bmp.Save("C:\\Users\\natha\\Documents\\Github\\Synthwave\\WorldMap.png", ImageFormat.Png);
    }

    static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    static float Noise(float x, float y, Random r)
    {
        return (float)(
            System.Math.Sin(x * 10.123 + r.NextDouble() * 10) *
            System.Math.Cos(y * 12.321 + r.NextDouble() * 10)
        ) * 0.5f + 0.5f;
    }

    static float Distance(float x, float y)
    {
        return MathF.Sqrt(x * x + y * y);
    }

    static float Clamp01(float v)
    {
        return MathF.Max(0, MathF.Min(1, v));
    }
}