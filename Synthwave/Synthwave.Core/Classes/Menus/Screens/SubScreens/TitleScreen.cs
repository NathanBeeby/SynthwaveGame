using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Menus.Core;

namespace Synthwave.Core.Classes.Menus.Screens.SubScreens;

public class TitleScreen : GameScreen
{
    private Cutscene _cutscene;

    public override void LoadContent()
    {
      //  _cutscene = new ImageCutscene(AssetLoader.Load<Texture2D>("title"),3f);
    }

    public override void Update(GameTime gameTime)
    {
        _cutscene.Update(gameTime);

        if (_cutscene.IsFinished)
        {
            ScreenManager.ChangeScreen(new LoadingScreen(async () =>
            {
                // load assets here
            }));
        }
    }

    public override void Draw(GameTime gameTime)
    {
        _cutscene.Draw(ScreenManager.SpriteBatch);
    }
}
