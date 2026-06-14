using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;

namespace Synthwave.Core.Classes.Controls.UI;

public class UISlider : UIElement
{
    public float Min = 0f;
    public float Max = 1f;
    public float Value = 0.5f;

    public Action<float> OnChanged;

    private bool _dragging;

    public override void Update(GameTime gameTime)
    {
        var mouse = Mouse.GetState();

        var barRect = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)Size.X,
            (int)Size.Y);

        bool hovered = barRect.Contains(mouse.Position);
        bool pressed = mouse.LeftButton == ButtonState.Pressed;

        if (pressed && hovered)
            _dragging = true;

        if (!pressed)
            _dragging = false;

        if (_dragging)
        {
            float t = MathHelper.Clamp(
                (mouse.X - Position.X) / Size.X,
                0f,
                1f);

            float newValue = MathHelper.Lerp(Min, Max, t);

            if (Math.Abs(newValue - Value) > 0.0001f)
            {
                Value = newValue;
                OnChanged?.Invoke(Value);
            }
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // background bar
         spriteBatch.Draw(TextureStore.Pixel, Position, Color.DarkSlateGray);

        float t = (Value - Min) / (Max - Min);
        float handleX = Position.X + t * Size.X;

        // handle
         spriteBatch.Draw(TextureStore.Pixel, new Vector2(handleX, Position.Y), Color.White);
    }
}