using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Controls.UI;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Menus.Core;

public class MenuScreen : GameScreen
{
    protected List<UIElement> Elements = [];

    public override void Update(GameTime gameTime)
    {
        foreach (var e in Elements)
            e.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        ScreenManager.GraphicsDevice.Clear(Color.Black);

        var spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();

        foreach (var e in Elements)
            e.Draw(spriteBatch);

        spriteBatch.End();
    }
}
