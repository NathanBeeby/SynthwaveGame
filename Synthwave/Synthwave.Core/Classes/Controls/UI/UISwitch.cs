using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public class UISwitch : UIElement
{
    public bool IsOn;
    public Action<bool> OnChanged;

    private bool _hovered;
    private bool _wasPressed;

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        var bounds = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y);

        _hovered = bounds.Contains(mouse.Position);

        bool pressed = mouse.LeftButton == ButtonState.Pressed;

        if (_hovered && pressed && !_wasPressed)
        {
            IsOn = !IsOn;
            OnChanged?.Invoke(IsOn);
        }

        _wasPressed = pressed;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color bg = IsOn ? Color.CornflowerBlue : Color.DarkGray;

        // knob position (visual only)
        float t = IsOn ? 1f : 0f;

         spriteBatch.Draw(TextureStore.Pixel, Position, bg);
    }
}