using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.HUD;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Synthwave.Core;

public class SynthwaveGame : Game
{
    #region Properties
    private readonly GraphicsDeviceManager _graphics;
    private Camera3D _camera;
    private InputHandler _input;
    private SynthwaveWorld _world;
    private HUD _hud;
    private SpriteBatch _spriteBatch;

    public readonly static bool IsMobile = OperatingSystem.IsAndroid() || OperatingSystem.IsIOS();
    public readonly static bool IsDesktop = OperatingSystem.IsMacOS() || OperatingSystem.IsLinux() || OperatingSystem.IsWindows();
    #endregion

    #region Constructor
    public SynthwaveGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Services.AddService(typeof(GraphicsDeviceManager), _graphics);

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

        _input = new InputHandler();
        _camera = new Camera3D(GraphicsDevice);
        _world = new SynthwaveWorld();
        _world.Initialize(GraphicsDevice, _camera);
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _hud = new HUD();

        _hud.Load(Content);
        // worldLoader ??= new WorldLoader(Content, GraphicsDevice);
      //  modelLoader ??= new ModelLoader(Content);
    }

    protected override void UnloadContent()
    {
        base.UnloadContent();

        // worldLoader.Update(camera.Position);
    }
    #endregion

    protected override void Update(GameTime gameTime)
    {
        _input.Update();

        if (_input.IsKeyDown(Keys.Escape)) Exit();

        _camera.Update(gameTime, _input);

        // Note: world.Update now also takes InputHandler for debug controls
        _world.Update(gameTime, _camera, _input);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(10, 0, 30));

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;
        GraphicsDevice.RasterizerState = RasterizerState.CullNone;

        _world.Draw(GraphicsDevice, _camera);
        _hud.Draw(_spriteBatch,_camera.Vehicle);

        base.Draw(gameTime);
    }
    #endregion
}

/*
Issues to fix:

Terrain too sharp — smoother noise, AND terrain must be flat along the road corridor (flatten within road half-width + margin)
Car physics — need CurrentSpeed, Acceleration, gears (1-6), top speed, W=accelerate, S=brake/reverse, A/D = steer (yaw change proportional to speed), not strafe
Q/E = turn view, R/V = camera up/down, Space = fly up, Shift = fly down in debug
Roads overlap — need road deduplication at intersections (blend/merge geometry). The real fix is: roads are solid-filled yellow, raised above terrain flatten zone, and where two roads cross the builder skips re-adding geometry already covered
Roads wider × 2, yellow colour, curbs either side (raised edge strips), no curbs on roundabouts
Ground grid: glowing pulsing lines — drive the colour via a PulseTime uniform animated each frame, encode terrain biome into line colour (pink=default, green=grassland, yellow=sand, blue=water), rebuild the grid each frame since line colours depend on world position + biome
 
-	Add First Person & Third Person View
-	Add NOS and Drifting ability + Accelerations.

-	Add Weather Controls
-	Add in different Biomes 
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
-	Add Weather
-	When not raining, add rubber tyre on floor
-	Points for Nitros 
-	Ability to drift
-	Add secret level for secret mission success
-	Thunder & Lightning / Weather switches

 */