using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIButtonCarousel : UIElement
{
    public List<string> Items = [];
    public int Index;

    public Action<int, string> OnChanged;

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
            // split into left/right halves
            bool leftSide = mouse.X < Position.X + Size.X / 2;

            if (leftSide)
                Index--;
            else
                Index++;

            if (Items.Count > 0)
                Index = (Index + Items.Count) % Items.Count;

            OnChanged?.Invoke(Index, Items[Index]);
        }

        _wasPressed = pressed;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        string label = Items.Count > 0 ? Items[Index] : "None";

        // draw main box
         spriteBatch.Draw(TextureStore.Pixel, Position, Color.Black);

        // draw label underneath (needs SpriteFont)
    }
}