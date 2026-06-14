using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Graphics.HUD;
using Synthwave.Core.Classes.Menus.Core;
using Synthwave.Core.Classes.Particles;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;
using System;

namespace Synthwave.Core.Classes.Menus.Screens;

public class GameplayScreen : GameScreen
{
    #region Properties
    private InputHandler _input;
    private WeatherSystem _weather;
    private Camera3D _camera;
    private SynthwaveWorld _world;
    private HUD _hud;
    private SpriteBatch _spriteBatch;
    private BloomManager _bloom;
    private ParticleManager _particleManager;
    private GraphicsDevice _graphics;
    #endregion

    #region Constructor
    public GameplayScreen()
    {

    }
    #endregion

    #region Initialization
    public override void Initialize()
    {
        base.Initialize();

    }

    public override void LoadContent()
    {
        _graphics = Services.GetService<GraphicsDevice>();
        _input = Services.GetService<InputHandler>();
        _weather = Services.GetService<WeatherSystem>();
        _camera = Services.GetService<Camera3D>();
        _world = Services.GetService<SynthwaveWorld>();
        _hud = Services.GetService<HUD>();
        _bloom = Services.GetService<BloomManager>();
        _spriteBatch = Services.GetService<SpriteBatch>();
        _particleManager = Services.GetService<ParticleManager>();
    }
    #endregion

    #region Methods
    public override void Update(GameTime gameTime)
    {
        _input.Update();
        if (_input.IsKeyDown(Keys.Escape)) Environment.Exit(0);
        _weather.Update(gameTime);
        _camera.Update(gameTime, _input, _weather);
        _world.Update(gameTime, _camera, _input, _weather);
    }

    public override void Draw(GameTime gameTime)
    {
        // SynthwaveWorld.Draw handles its own render targets, post-FX pipeline
        // (rain → snow → fog → tyre marks → speed blur) and returns the
        // fully composited texture.  Do NOT wrap it in BeginScene/EndScene —
        // that would fight the world's internal SetRenderTarget calls.
        RenderTarget2D worldTexture = _world.Draw(_graphics, _camera, gameTime, _weather);
        if (worldTexture == null) return;
        // Draw particles on top of the composited world texture.
        // They need to go into the world pipeline if they should receive post-FX;
        // here they are drawn screen-space on top (simpler and usually fine).
        _particleManager.Draw(_graphics, _camera, TextureStore.Pixel);

        // ── Blit world texture to back buffer ────────────────────────────────
        // The world's final pass already rendered to the back buffer when
        // output == null in ApplyFullScreen, so worldTexture holds the
        // second-to-last ping-pong buffer.  We still need to present it.
        _graphics.SetRenderTarget(null);
        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
            SamplerState.LinearClamp, null, null);
        _spriteBatch.Draw(
            worldTexture,
            new Rectangle(0, 0, _graphics.Viewport.Width, _graphics.Viewport.Height),
            Color.White);
        _spriteBatch.End();

        // ── HUD always last — screen space ───────────────────────────────────
        _hud.Draw(_spriteBatch, _camera.Vehicle, _weather);
    }
    //public override void Draw(GameTime gameTime)
    //{
    //    _graphics.Clear(Color.Lerp(
    //        new Color(10, 0, 30),
    //        Color.Black,
    //        1f - _weather.Visibility));

    //    _graphics.DepthStencilState = DepthStencilState.Default;
    //    _graphics.BlendState = BlendState.Opaque;
    //    _graphics.RasterizerState = RasterizerState.CullNone;

    //    // ─────────────────────────────
    //    // BLOOM START
    //    // ─────────────────────────────
    //    _bloom.BeginScene();

    //    // ✔ WORLD now RETURNS a texture instead of presenting
    //    RenderTarget2D worldTexture = _world.Draw(_graphics, _camera, gameTime, _weather);

    //    // Optional: if particles use world space, keep here
    //    _particleManager.Draw(_graphics, _camera, TextureStore.Pixel);

    //    _bloom.EndScene();

    //    _bloom.ExtractBrightPass(_spriteBatch);
    //    _bloom.BlurHorizontal(_spriteBatch);
    //    _bloom.BlurVertical(_spriteBatch);
    //    _bloom.Combine(_spriteBatch);

    //    // ─────────────────────────────
    //    // HUD LAST (always screen-space)
    //    // ─────────────────────────────
    //    _hud.Draw(_spriteBatch, _camera.Vehicle, _weather);
    //}
    #endregion
}
