using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Core;

public static class TextureStore
{
    private static Texture2D _pixel;
    private static bool _initialized;

    public static Texture2D Pixel => _pixel;

    /// <summary>
    /// Must be called once during Game.LoadContent()
    /// </summary>
    public static void Initialize(GraphicsDevice graphicsDevice)
    {
        if (_initialized)
            return;

        _pixel = new Texture2D(graphicsDevice, 1, 1);
        _pixel.SetData(new[] { Color.White });

        _initialized = true;
    }

    public static void Dispose()
    {
        _pixel?.Dispose();
        _pixel = null;
        _initialized = false;
    }
}