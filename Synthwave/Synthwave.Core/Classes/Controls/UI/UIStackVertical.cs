using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Controls.UI;

public class UIStackVertical : UIElement
{
    public List<UIElement> Children = [];
    public float Spacing = 10f;

    public override void Update(GameTime gameTime)
    {
        float y = Position.Y;

        foreach (var child in Children)
        {
            child.Position = new Vector2(Position.X, y);
            y += child.Size.Y + Spacing;

            child.Update(gameTime);
        }
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        foreach (var child in Children)
            child.Draw(spriteBatch);
    }
}