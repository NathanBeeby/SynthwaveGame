using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Input;

namespace Synthwave.Core.Classes.Menus.Core;

public abstract class GameScreen
{
    protected ScreenManager ScreenManager;
    protected GameServiceContainer Services => ScreenManager.Services;
    protected InputContext Input => Services.GetService<InputManager>().GetState();

    public bool IsLoaded { get; private set; }

    public void SetManager(ScreenManager manager) => ScreenManager = manager;
    
    public virtual void Initialize() { }

    public virtual void LoadContent() { }
    public virtual void UnloadContent() { }

    public virtual void Update(GameTime gameTime) { }
    public virtual void Draw(GameTime gameTime) { }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public void Load()
    {
        if (IsLoaded) return;
        LoadContent();
        IsLoaded = true;
    }

    public void Unload()
    {
        if (!IsLoaded) return;
        UnloadContent();
        IsLoaded = false;
    }
}

/*
 Usage in Screen:
public override void Update(GameTime gameTime)
{
    if (Input.IsBlocked)
        return;

    if (Input.Keyboard.IsKeyDown(Keys.Escape))
        ScreenManager.ChangeScreen(new MainMenuScreen());
} 
 */