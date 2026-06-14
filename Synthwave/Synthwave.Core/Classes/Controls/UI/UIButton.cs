using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Interfaces;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIButton : UIElement, IFocusable
{
    public string Text;
    public Action OnClick;

    public bool IsFocused { get; set; }

    private bool _hovered;

    public void OnFocus() { }
    public void OnUnfocus() { }

    public void OnConfirm() => OnClick?.Invoke();

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        var bounds = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y);

        _hovered = bounds.Contains(mouse.Position);

        if (_hovered && mouse.LeftButton == ButtonState.Pressed)
            OnClick?.Invoke();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float time = (float)DateTime.Now.TimeOfDay.TotalSeconds;

        float glow =
            (_hovered || IsFocused)
            ? UIGlow.Pulse(time)
            : 0.25f;

        Color baseColor = Color.Cyan;
        Color final = UIGlow.Neon(baseColor, glow);

        // draw neon rectangle
         spriteBatch.Draw(TextureStore.Pixel, Position, final);
    }
}