using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Graphics.HUD;
using Synthwave.Core.Classes.Menus.Core;
using Synthwave.Core.Classes.Menus.Screens;
using Synthwave.Core.Classes.Particles;
using Synthwave.Core.Classes.Vehicle;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;
using Synthwave.Core.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Synthwave.Core;

public class SynthwaveGame : Game
{
    #region Properties
    private GameServiceContainer _services;
    private VehicleController _vehicle;
    private ScreenManager _screenManager;

    private readonly GraphicsDeviceManager _graphics;
    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
    public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();

    private Effect _brightEffect;
    private Effect _blurEffect;
    private Effect _combinedEffect;
    #endregion

    #region Constructor
    public SynthwaveGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Services.AddService(typeof(GraphicsDeviceManager), _graphics);
        _services = Services;
        Content.RootDirectory = "Content";
        Window.Title = "SynthWave";
        _graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
    }
    #endregion

    #region Methods
    #region Initialization
    protected override void Initialize()
    {
        base.Initialize();

        List<CultureInfo> cultures = LocalizationManager.GetSupportedCultures();
        var languages = new List<CultureInfo>();
        for (int i = 0; i < cultures.Count; i++)
        {
            languages.Add(cultures[i]);
        }

        var selectedLanguage = LocalizationManager.DEFAULT_CULTURE_CODE;
        LocalizationManager.SetCulture(selectedLanguage);

    }

    protected override void LoadContent()
    {
        base.LoadContent();
        _services.AddService(GraphicsDevice);
        // Core MonoGame services
        _services.AddService(Content);

        _services.AddService(new InputHandler());

        var spriteBatch = new SpriteBatch(GraphicsDevice);
        _screenManager = new ScreenManager(_services);
        var Weather = new WeatherSystem();

        _services.AddService(spriteBatch);
        _services.AddService(Weather);

        // Your systems
        var world = new SynthwaveWorld();
        var Vehicle = new VehicleController(_services);
        _services.AddService(Vehicle);
        var camera = new Camera3D(GraphicsDevice, _screenManager);
        world.Initialize(Content, GraphicsDevice, camera);
        var debugFont = Content.Load<SpriteFont>("Fonts/Hud");



        _services.AddService(new DebugOverlay(debugFont));

        _brightEffect = Content.Load<Effect>("Shaders/BrightPassParticle");
        _blurEffect = Content.Load<Effect>("Shaders/BlurParticle");
        _combinedEffect = Content.Load<Effect>("Shaders/CombinedParticle");

        var BloomMgr = new BloomManager(GraphicsDevice, _brightEffect, _blurEffect, _combinedEffect);
        _services.AddService(BloomMgr);


        var particleManager = new ParticleManager();
        _services.AddService(particleManager);
        _services.AddService(camera);
        _services.AddService(world);

        var hud = new HUD();
        hud.Load(Content, Vehicle, camera, Weather);
        _services.AddService(hud);

        _screenManager.ChangeScreen(new GameplayScreen());
        _screenManager.LoadContent();
        // Screen system

    }

    protected override void UnloadContent()
    {
        base.UnloadContent();
        _screenManager?.UnloadContent();
        // worldLoader.Update(camera.Position);
    }
    #endregion

    protected override void OnActivated(object sender, EventArgs args)
    {
        base.OnActivated(sender, args);
        _screenManager?.OnActivated(sender, args);
        // redraw everything on activated

    }

    protected override void OnDeactivated(object sender, EventArgs args)
    {
        base.OnDeactivated(sender, args);
        _screenManager?.OnDeactivated(sender, args);
    // Stop everything if deactivated
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        base.OnExiting(sender, args);
        _screenManager?.OnExiting(sender, args);
        // implement Exiting actions (e.g. Save Game)
    }

    protected override void BeginRun()
    {
        base.BeginRun();
        _screenManager?.BeginRun();
    }

    protected override bool BeginDraw()
    {
        _screenManager?.BeginDraw();
        return base.BeginDraw();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _screenManager?.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
        _screenManager?.Draw(gameTime);
    }

    protected override void EndRun()
    {
        base.EndRun();
        _screenManager?.EndRun();
    }

    protected override void EndDraw()
    {
        base.EndDraw();
        _screenManager?.EndDraw();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        _screenManager?.Dispose(disposing);
    }
    #endregion
}

/*
Issues to fix:

Terrain too sharp — smoother noise, AND terrain must be flat along the road corridor (flatten within road half-width + margin)
Roads overlap — need road deduplication at intersections (blend/merge geometry). The real fix is: roads are solid-filled yellow, raised above terrain flatten zone, and where two roads cross the builder skips re-adding geometry already covered
Roads wider × 2, yellow colour, curbs either side (raised edge strips), no curbs on roundabouts


-	Collectable Coins
-	Add in Shader & Include Rubber on floor in dry, or rain on floor / Puddles when raining.
-   Add Shader for reflective roads, road center line and side reservations in neon yellow and neon light blue
-	Add algorithm for roads, Implement neon road markings at the side of the roads and add a centrer line road markings.
-   Position user always on the road.
-	IMPLEMENT STREET LIGHTS + NEON LED

Mission Types:
-	Collections & mission objects
-	Pit Lanes (Tyre ware)
-	Hit & Run elements
-	Collect Coins
-	Purchase new vehicles or parts (Go to garage)
-	Vibrate on hit on phone
-	Points for Nitros 
-	Ability to drift
-	Add secret level for secret mission success
-	Thunder & Lightning / Weather switches


TODO LATER:
    - Improve Weather System Shaders
    - Improve ground rendering

 */

// Crate Menu System
// Create HUD system with movable view.
// Create Power Up system
// Create Memory Storage System
// Implement Achievement System & Achievement Notifications
// Implement Shop System

// Move over MVVM Service based design.
// Move all of game code and SynthwaveWorld into a Game class.
/*
 Potential Additions:

🔥 Transition types
slide left/right
zoom blur
pixel dissolve
🔥 Input layers
UI layer
gameplay layer
modal dialogs
🔥 Debug console (Unity-style)
runtime commands
teleport player
spawn objects
toggle systems
🔥 Scene graph debugger
visual entity tree
live collision debug view 

 🔥 HDR lighting pipeline (real engine-style)
🌈 Color grading (LUT system for synthwave look)
💡 Light volume system (particles actually illuminate world)
🎬 Camera bloom + motion blur combo
🧠 Per-material emissive control system
 */


/*
 
 public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;

    protected override void Initialize()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _screenManager = new ScreenManager(GraphicsDevice, Content);

        _screenManager.ChangeScreen(new TitleScreen());
    }

    protected override void Update(GameTime gameTime)
    {
        _screenManager.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _screenManager.Draw(gameTime);

        base.Draw(gameTime);
    }
}
 
 */

