using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Synthwave.Core.Classes.Core._3DModel;

public class MaterialFactory
{
    private readonly ContentManager _content;

    private Effect _standardEffect;
    private Effect _emissiveEffect;

    // Textures
    private Texture2D _carDiffuse;

    private Texture2D _lampDiffuse;
    private Texture2D _lampEmissive;

    private Texture2D _buildingDiffuse;
    private Texture2D _buildingEmissive;

    private Texture2D _yukkaDiffuse;

    public MaterialFactory(ContentManager content, GraphicsDevice gd)
    {
        _content = content;
        Load(gd);
    }

    private void Load(GraphicsDevice graphicsDevice)
    {
        
        _standardEffect = _content.Load<Effect>("Shaders/StandardShader");
        _emissiveEffect = _content.Load<Effect>("Shaders/EmissiveShader");

        _carDiffuse = CreateNoisyTexture(graphicsDevice, 512, 512, new Color(180, 0, 0), 10);

        _lampDiffuse = CreateSolidTexture(graphicsDevice, 256, 256, new Color(200, 200, 200));
        _lampEmissive = CreateRadialGlow(graphicsDevice, 256, Color.Yellow);

        _buildingDiffuse = CreateBuildingTexture(graphicsDevice, 512, 512);
        _buildingEmissive = CreateWindowsTexture(graphicsDevice, 512, 512);

        _yukkaDiffuse = CreateNoisyTexture(graphicsDevice, 256, 256, new Color(40, 120, 60), 25);
    }
    public Material CreateCarMaterial()
    {
        return new Material
        {
            Effect = _standardEffect,
            Diffuse = _carDiffuse,

            UseEmissive = false,
            EmissiveStrength = 0f
        };
    }

    public Material CreateLampMaterial()
    {
        return new Material
        {
            Effect = _emissiveEffect,

            Diffuse = _lampDiffuse,
            Emissive = _lampEmissive,

            UseEmissive = true,
            EmissiveStrength = 3.5f
        };
    }

    public Material CreateBuildingMaterial()
    {
        return new Material
        {
            Effect = _emissiveEffect,

            Diffuse = _buildingDiffuse,
            Emissive = _buildingEmissive,

            UseEmissive = true,
            EmissiveStrength = 1.5f
        };
    }

    public Material CreateYukkaMaterial()
    {
        return new Material
        {
            Effect = _standardEffect,
            Diffuse = _yukkaDiffuse,

            UseEmissive = false,
            EmissiveStrength = 0f
        };
    }

    Texture2D CreateSolidTexture(GraphicsDevice graphicsDevice, int width, int height, Color color)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int i = 0; i < data.Length; i++)
            data[i] = color;

        texture.SetData(data);
        return texture;
    }

    Texture2D CreateNoisyTexture(GraphicsDevice graphicsDevice, int width, int height, Color baseColor, int variance)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        Random r = new Random();

        for (int i = 0; i < data.Length; i++)
        {
            int v = r.Next(-variance, variance);
            data[i] = new Color(
                MathHelper.Clamp(baseColor.R + v, 0, 255),
                MathHelper.Clamp(baseColor.G + v, 0, 255),
                MathHelper.Clamp(baseColor.B + v, 0, 255)
            );
        }

        texture.SetData(data);
        return texture;
    }

    Texture2D CreateRadialGlow(GraphicsDevice graphicsDevice, int size, Color centerColor)
    {
        Texture2D texture = new Texture2D(graphicsDevice, size, size);
        Color[] data = new Color[size * size];

        Vector2 center = new Vector2(size / 2f);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist = Vector2.Distance(center, new Vector2(x, y)) / (size / 2f);
                float intensity = MathHelper.Clamp(1f - dist, 0, 1);

                Color c = centerColor * intensity;
                data[y * size + x] = c;
            }
        }

        texture.SetData(data);
        return texture;
    }

    Texture2D CreateBuildingTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool brick = ((x / 32 + y / 16) % 2 == 0);
                data[y * width + x] = brick
                    ? new Color(120, 120, 130)
                    : new Color(100, 100, 110);
            }
        }

        texture.SetData(data);
        return texture;
    }

    Texture2D CreateWindowsTexture(GraphicsDevice graphicsDevice, int width, int height)
    {
        Texture2D texture = new Texture2D(graphicsDevice, width, height);
        Color[] data = new Color[width * height];

        Random r = new Random();

        for (int i = 0; i < data.Length; i++)
            data[i] = Color.Black;

        for (int y = 0; y < height; y += 16)
        {
            for (int x = 0; x < width; x += 16)
            {
                bool lit = r.NextDouble() > 0.5;

                Color c = lit ? Color.Yellow : Color.Black;

                for (int j = 0; j < 16; j++)
                {
                    for (int i = 0; i < 16; i++)
                    {
                        int px = x + i;
                        int py = y + j;

                        if (px < width && py < height)
                            data[py * width + px] = c;
                    }
                }
            }
        }

        texture.SetData(data);
        return texture;
    }
}