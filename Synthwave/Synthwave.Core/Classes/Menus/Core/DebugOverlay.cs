using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Input;

namespace Synthwave.Core.Classes.Menus.Core;

public class DebugOverlay
{
    private readonly SpriteFont _font;

    public bool Enabled = true;

    public DebugOverlay(SpriteFont font)
    {
        _font = font;
    }

    public void Draw(SpriteBatch spriteBatch, GameTime gameTime, GameServiceContainer services)
    {
        if (!Enabled) return;

        var fps = 1f / (float)gameTime.ElapsedGameTime.TotalSeconds;

        var input = services.GetService<InputManager>().GetState();

        string text =
$@"DEBUG OVERLAY
FPS: {fps:0}
Time: {gameTime.TotalGameTime.TotalSeconds:0.00}
Input Blocked: {input.IsBlocked}
Screen: {services.GetService<ScreenManager>().GetType().Name}";

        spriteBatch.DrawString(_font, text, new Vector2(10, 10), Color.Lime);
    }
}