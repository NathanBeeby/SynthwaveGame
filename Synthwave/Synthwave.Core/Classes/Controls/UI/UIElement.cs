using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Controls.UI;

public abstract class UIElement
{
    public Vector2 Position;
    public Vector2 Size;

    public abstract void Update(GameTime gameTime);
    public abstract void Draw(SpriteBatch spriteBatch);
}


/*
 * POTENTIAL UPGRADES:
 🌟 1. UI layout system
vertical / horizontal stack panels
auto scaling menus
🎬 2. UI animation system
fade in/out
slide menus
spring transitions
🧠 3. Input abstraction layer
unify mouse + touch + controller
🎨 4. Skinning system
neon UI theme
glass UI theme
animated hover glow
 
 */