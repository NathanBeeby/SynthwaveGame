using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Menus.Core.Transitions;
using System;

namespace Synthwave.Core.Classes.Menus.Core;

public class ScreenManager(GameServiceContainer services)
{
    #region Core MonoGame Context
    public GameServiceContainer Services = services;
    public GraphicsDevice GraphicsDevice = services.GetService<GraphicsDevice>();
    public SpriteBatch SpriteBatch = services.GetService<SpriteBatch>();
    public ContentManager Content = services.GetService<ContentManager>();
    public InputManager Input => services.GetService<InputManager>();
    #endregion

    #region Properties
    private GameScreen _currentScreen;
    private GameScreen _nextScreen;

    private ScreenTransition _transition;
    private bool _isTransitioning;

    public GameScreen Current => _currentScreen;
    #endregion

    #region Overridable Events
    public void LoadContent()
    {
        _currentScreen.Initialize();
        _currentScreen.Load();
        _currentScreen.LoadContent();

    }

    public void UnloadContent()
    {
        _currentScreen.UnloadContent();
    }

    public void OnActivated(object sender, EventArgs args)
    {
    }

    public void OnDeactivated(object sender, EventArgs args)
    {
    }

    public void OnExiting(object sender, ExitingEventArgs args)
    {
    }

    public void BeginRun()
    {

    }

    public bool BeginDraw()
    {
        return false;
    }

    public void EndRun()
    {

    }

    public void EndDraw()
    {

    }

    public void Dispose(bool disposing)
    {

    }
    #endregion



    #region Methods
    public void ChangeScreen(GameScreen newScreen)
    {
        if (_isTransitioning) return;

        var graphics = Services.GetService<GraphicsDevice>();

        // capture current frame
        var snapshot = new RenderTarget2D(graphics, graphics.PresentationParameters.BackBufferWidth, graphics.PresentationParameters.BackBufferHeight);

        _isTransitioning = true;

        _transition = new ScreenTransition();
        //.Draw(TextureStore.Pixel, fullScreenRect, Color.Black * transition.Alpha);



        _currentScreen = newScreen; 
        _nextScreen = newScreen;
        _nextScreen.SetManager(this);
    }
    //public void ChangeScreen(GameScreen newScreen)
    //{
    //    _nextScreen = newScreen;

    //    _currentScreen?.OnExit();
    //    _currentScreen?.Unload();

    //    _currentScreen = _nextScreen;
    //    _nextScreen = null;

    //    _currentScreen.SetManager(this);
    //    _currentScreen.Load();
    //    _currentScreen.OnEnter();
    //}

    public void Update(GameTime gameTime)
    {
        //if (_isTransitioning)
        //{
        //   // _transition.Update(gameTime);

        //    if (_transition.IsFinished)
        //    {
        //_currentScreen?.Unload();
        //if (_nextScreen == null) return;
        //_currentScreen = _nextScreen;

        //_currentScreen.Load();
        //_currentScreen.OnEnter();

        //_nextScreen = null;

        //  ((FadeTransition)_transition).StartIn();
        //    }

        //    return;
        //}

        _currentScreen?.Update(gameTime);
    }

    public void Draw(GameTime gameTime)
    {
        _currentScreen?.Draw(gameTime);

        //  if (_isTransitioning) _transition.Draw(Services.GetService<SpriteBatch>());
    }
    #endregion
}