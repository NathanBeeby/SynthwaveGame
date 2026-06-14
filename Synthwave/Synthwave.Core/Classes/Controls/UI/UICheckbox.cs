using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public class UICheckbox : UIElement
{
    public bool IsChecked;

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
            IsChecked = !IsChecked;
            OnChanged?.Invoke(IsChecked);
        }

        _wasPressed = pressed;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        Color color = IsChecked ? Color.LimeGreen : Color.DarkGray;

        if (_hovered) color = Color.Lerp(color, Color.White, 0.3f);

        // draw box (replace with texture later)
         spriteBatch.Draw(TextureStore.Pixel, Position, color);
    }
}