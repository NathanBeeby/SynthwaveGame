using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Controls.UI;
using Synthwave.Core.Classes.Menus.Core.Transitions;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Menus.Core;

public abstract class UIScreen
{
    public List<UIElement> Elements = [];
    public UIFocusManager Focus = new();
    public UIInput Input = new();
    public ScreenTransition Transition = new();

    public virtual void Load() { }
    public virtual void Unload() { }

    public virtual void Update(GameTime gameTime)
    {
        Input.Update();
        Focus.Update(Input.State);

        foreach (var e in Elements)
            e.Update(gameTime);
    }

    public virtual void Draw(SpriteBatch spriteBatch)
    {
        foreach (var e in Elements)
            e.Draw(spriteBatch);
    }
}
