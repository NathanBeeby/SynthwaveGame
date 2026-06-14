using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Menus.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthwave.Core.Classes.Menus.Screens.SubScreens;

public class LoadingScreen(Func<Task> loadTask) : GameScreen
{
    private float _progress;

    private Func<Task> _loadTask = loadTask;

    public override async void OnEnter()
    {
        await _loadTask();

     //   ScreenManager.ChangeScreen(new MainMenuScreen());
    }

    public override void Draw(GameTime gameTime)
    {
        // draw loading bar using _progress
    }
}
