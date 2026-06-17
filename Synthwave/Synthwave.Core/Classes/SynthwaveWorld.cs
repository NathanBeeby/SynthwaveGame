using Synthwave.Core.Classes.Core._3DModel;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.Renderer;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Roads;
using Synthwave.Core.Classes.World.Sky;
using Synthwave.Core.Classes.World.Terrain;
using Synthwave.Core.Classes.World.Weather;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;

namespace Synthwave.Core.Classes;

public class SynthwaveWorld
{
    #region Properties
    public TerrainSystem Terrain;
    public RoadSplineSystem Roads;
    public CityBlockGenerator City;
    public TrafficSystem Traffic;
    public LightingSystem Lighting;
    public SkySystem Sky;
    public BloomRenderer Bloom;
    public InfiniteWorldManager Infinite;
    public SynthwaveGroundRenderer Ground;
    public MaterialFactory mFactory;
    public ModelLoader Models;

    private BasicEffect _terrainFx;
    private BasicEffect _carFx;
    private BasicEffect _blockFx;

    private RoadRenderer _roadRenderer;

    private VertexPositionTexture[] _fullscreenQuad;

    private RenderTarget2D _rainRT;
    private RenderTarget2D _snowRT;
    private RenderTarget2D _fogRT;
    private RenderTarget2D _speedRT;
    private RenderTarget2D _postRT;
    private RenderTarget2D _tyreMarkRT;

    private Effect _rainEffect;
    private Effect _rainSkyEffect;
    private Effect _snowEffect;
    private Effect _fogEffect;
    private Effect _speedEffect;
    private Effect _reflectiveRoadFx;
    private Effect _neonMaskEffect;
    private Effect _postEffect;
    private Effect _tyreMarkEffect;

    private readonly VertexPositionColor[] _carVerts = new VertexPositionColor[2];
    private readonly VertexPositionColor[] _blockVerts = new VertexPositionColor[2];

    private RenderTarget2D _sceneRT;

    private GraphicsDevice _device;
    private SpriteBatch _spriteBatch;

    private bool _disablePostFX = false;
    #endregion
    private void InitializeWorld(GraphicsDevice device, Camera3D camera)
    {
        Terrain = new TerrainSystem(512, 512);
        Roads = new RoadSplineSystem();
        City = new CityBlockGenerator();
        Traffic = new TrafficSystem();
        Lighting = new LightingSystem();
        Sky = new SkySystem();

        Sky.Initialize(device);

        Bloom = new BloomRenderer { Camera = camera };
        Bloom.Initialize(device);

        Ground = new SynthwaveGroundRenderer();
        Ground.Initialize(device);

        Infinite = new InfiniteWorldManager(device, Terrain);

        Roads.Generate();
        City.Generate(Roads, Terrain);
        Traffic.Spawn(Roads, Terrain);
    }

    private void InitializeEffects(GraphicsDevice device)
    {
        _terrainFx = MakeEffect(device);
        _carFx = MakeEffect(device);
        _blockFx = MakeEffect(device);
    }

    private void InitializeRenderTargets(GraphicsDevice device)
    {
        int w = device.PresentationParameters.BackBufferWidth;
        int h = device.PresentationParameters.BackBufferHeight;

        _rainRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        _snowRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        _fogRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        _speedRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        _postRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
        _tyreMarkRT = new RenderTarget2D(device, w, h, false, SurfaceFormat.Color, DepthFormat.None);
    }

    private void InitializeQuads()
    {
        _fullscreenQuad =
        [
            new VertexPositionTexture(new Vector3(-1,  1, 0), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
            new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3( 1, -1, 0), new Vector2(1, 1)),
        ];
    }

    private void InitializeEffects(ContentManager content)
    {
        _rainEffect = content.Load<Effect>("Shaders/RainPixelShader");
        _rainSkyEffect = content.Load<Effect>("Shaders/RainSky");
        _snowEffect = content.Load<Effect>("Shaders/Snow");
        _fogEffect = content.Load<Effect>("Shaders/FogShader");
        _speedEffect = content.Load<Effect>("Shaders/SpeedEffect");
        _reflectiveRoadFx = content.Load<Effect>("Shaders/ReflectiveRoad");
        _neonMaskEffect = content.Load<Effect>("Shaders/NeonMask");
        _postEffect = content.Load<Effect>("Shaders/PostProcessingShader");
        _tyreMarkEffect = content.Load<Effect>("Shaders/TyreMark");
    }

    public void Initialize(ContentManager content, GraphicsDevice device, Camera3D camera)
    {
        _device = device;
        _spriteBatch = new SpriteBatch(device);

        InitializeWorld(device, camera);
        InitializeEffects(device);
        InitializeRenderTargets(device);
        InitializeQuads();
        InitializeEffects(content);

        _sceneRT = new RenderTarget2D(device,device.Viewport.Width,device.Viewport.Height,false,SurfaceFormat.Color,DepthFormat.Depth24);
        mFactory = new MaterialFactory(content, device);

        Models = new ModelLoader(content, device, mFactory);
        Models.LoadModels();
        Models.Populate(Roads, Terrain);

        _roadRenderer = new RoadRenderer(device, _reflectiveRoadFx);
        _roadRenderer.SetCamera(camera);

        device.SetRenderTarget(_tyreMarkRT);
        device.Clear(Color.Transparent);
        device.SetRenderTarget(null);
    }

    public void Update(GameTime gameTime, Camera3D camera, InputHandler input, WeatherSystem weather)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

#if DEBUG
        if (Debugger.IsAttached)
        {
            if (input.IsKeyDown(Keys.PageUp)) Sky.SkipTime(dt * 2f);
            if (input.IsKeyDown(Keys.PageDown)) Sky.SkipTime(-dt * 2f);
        }
#endif
        camera.SnapToTerrain(Terrain.GetHeight(camera.Position.X, camera.Position.Z));

        Infinite.Update(camera.Position, Roads);
        Traffic.Update(dt, weather, Terrain);
        Sky.Update(dt, weather);
        City.Update(weather);

        UpdatePuddles(dt, weather);

        Bloom.CameraPosition = camera.Position;
        Lighting.Update(Sky);
        Lighting.Apply(Bloom);
    }

    private void UpdatePuddles(float dt, WeatherSystem weather)
    {
        for (int x = 0; x < Terrain.Width; x++)
            for (int z = 0; z < Terrain.Length; z++)
            {
                float current = Terrain.GetWaterLevel(x, z);
                current += weather.CurrentWeather is WeatherType.Rain or WeatherType.HeavyRain ? dt * 0.05f : 0f;
                current = MathHelper.Clamp(current * 0.99f, 0f, 1f);
                Terrain.SetPuddle(x, z, current);
            }
    }

    private static BasicEffect MakeEffect(GraphicsDevice device) => new(device) { VertexColorEnabled = true, LightingEnabled = false };
    private void DrawTerrain(GraphicsDevice device, Camera3D camera, List<WorldChunk> chunks)
    {
        _terrainFx.View = camera.View;
        _terrainFx.Projection = camera.Projection;
        _terrainFx.World = Matrix.Identity;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.TerrainVB == null) continue;
            device.SetVertexBuffer(chunk.TerrainVB);

            // ── Solid pass (triangle list) ─────────────────────────────────
            device.Indices = chunk.TerrainIB;
            device.BlendState = BlendState.Opaque;
            foreach (var pass in _terrainFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0,
                    chunk.TerrainIB.IndexCount / 3);
            }

            // ── Wireframe grid pass (line list) ────────────────────────────
            if (chunk.GridIB == null) continue;
            device.Indices = chunk.GridIB;
            device.BlendState = BlendState.Additive;   // neon additive glow
            foreach (var pass in _terrainFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.LineList, 0, 0,
                    chunk.GridIB.IndexCount / 2);
            }
            device.BlendState = BlendState.Opaque;
        }
    }

    // Dashing and reflectivity now live entirely in ReflectiveRoad.fx + the road mesh's
    // per-vertex distance/UV data, so this is just a render pass — no CPU geometry building,
    // no timer-based blinking.
    private void DrawRoads(GraphicsDevice device, List<WorldChunk> chunks, WeatherSystem weather, float time)
    {
        device.BlendState = BlendState.Opaque;

        float wetness = MathHelper.Clamp(weather.RainAmount, 0f, 1f);

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.RoadVB == null) continue;
            _roadRenderer.Draw(chunk, time);
        }
    }

    public RenderTarget2D Draw(GraphicsDevice device, Camera3D camera, GameTime gameTime, WeatherSystem weather)
    {
        float time = (float)gameTime.TotalGameTime.TotalSeconds;

        device.SetRenderTarget(_sceneRT);
        device.Clear(new Color(10, 0, 30));
        device.DepthStencilState = DepthStencilState.Default; // requires a depth buffer that doesn't exist
        device.BlendState = BlendState.Opaque;
        device.RasterizerState = RasterizerState.CullCounterClockwise;

        Sky.DrawSky(device, camera);
        var chunks = Infinite.GetVisibleChunks(camera.Position);
        DrawTerrain(device, camera, chunks);
        Ground.Draw(camera, gameTime);
        DrawRoads(device, chunks, weather, time);
        foreach (var car in Traffic.Cars) DrawCarGPU(device, car);
        foreach (var block in City.Blocks) DrawBlockGPU(device, block);
        device.SetRenderTarget(null);

        if (_disablePostFX) return _sceneRT;

        RenderTarget2D[] pool = [_sceneRT, _rainRT, _snowRT, _fogRT, _postRT, _speedRT];
        int poolIdx = 0;

        RenderTarget2D Current() => pool[poolIdx % pool.Length];
        RenderTarget2D Next() => pool[(poolIdx + 1) % pool.Length];
        void Swap() => poolIdx++;

        // Rain
        if (weather.RainAmount > 0.01f)
        {
            SetParam(_rainSkyEffect, "SceneTex", (Texture)Current());
            SetParam(_rainSkyEffect, "Time", time);
            SetParam(_rainSkyEffect, "RainIntensity", weather.RainAmount);
            ApplyFullScreen(device, _rainSkyEffect, Current(), Next());
            Swap();

            SetParam(_rainEffect, "SceneTex", (Texture)Current());
            SetParam(_rainEffect, "RainIntensity", weather.RainAmount);
            SetParam(_rainEffect, "Time", time);
            ApplyFullScreen(device, _rainEffect, Current(), Next());
            Swap();
        }

        // Snow
        if (weather.SnowAmount > 0.01f)
        {
            SetParam(_snowEffect, "SceneTex", (Texture)Current());
            SetParam(_snowEffect, "SnowAmount", weather.SnowAmount);
            SetParam(_snowEffect, "Time", time);          // was missing — snow never fell
            SetParam(_snowEffect, "WindStrength", 0f);    // wire to your wind value if WeatherSystem has one
            ApplyFullScreen(device, _snowEffect, Current(), Next());
            Swap();
        }

        // Fog
        if (1f - weather.Visibility > 0.01f)
        {
            SetParam(_fogEffect, "SceneTex", (Texture)Current());
            SetParam(_fogEffect, "FogIntensity", 1f - weather.Visibility);
            SetParam(_fogEffect, "Time", time);           // was missing — fog never animated
            ApplyFullScreen(device, _fogEffect, Current(), Next());
            Swap();
        }

        // Tyre marks (always runs — _tyreMarkRT is pre-cleared)
        SetParam(_tyreMarkEffect, "SceneTex", (Texture)Current());
        SetParam(_tyreMarkEffect, "SkidMapTex", _tyreMarkRT);
        ApplyFullScreen(device, _tyreMarkEffect, Current(), Next());
        Swap();

        // Speed blur
        SetParam(_speedEffect, "SceneTex", (Texture)Current());
        SetParam(_speedEffect, "SpeedAmount", camera.Vehicle.State.CurrentSpeed / 1000f);
        SetParam(_speedEffect, "Time", time);
        ApplyFullScreen(device, _speedEffect, Current(), Next());
        Swap();
        SetParam(_postEffect, "SceneTex", (Texture)Current());
        SetParam(_postEffect, "Time", time);
        ApplyFullScreen(device, _postEffect, Current(), Next());
        Swap();

        return Current();
    }

    private static void SetParam(Effect effect, string name, Texture value) => effect.Parameters[name]?.SetValue(value);

    private static void SetParam(Effect effect, string name, float value) => effect.Parameters[name]?.SetValue(value);

    private void ApplyFullScreen(GraphicsDevice device, Effect effect, Texture2D input, RenderTarget2D output)
    {
        device.SetRenderTarget(output);
        device.Clear(Color.Transparent);
        device.DepthStencilState = DepthStencilState.None;
        device.BlendState = BlendState.Opaque;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(
                PrimitiveType.TriangleList, _fullscreenQuad, 0, 2);
        }

        device.SetRenderTarget(null);

        // Restore 3D state for next frame's scene pass.
        device.DepthStencilState = DepthStencilState.Default;
        device.BlendState = BlendState.Opaque;
    }

    public void TogglePostFX(bool enabled) => _disablePostFX = !enabled;

    private void DrawCarGPU(GraphicsDevice device, Car car)
    {
        _carVerts[0] = new VertexPositionColor(car.Position, Color.Cyan);
        _carVerts[1] = new VertexPositionColor(car.Position + Vector3.Up * 2, Color.Cyan);
        foreach (var pass in _carFx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.LineList, _carVerts, 0, 1);
        }
    }

    private void DrawBlockGPU(GraphicsDevice device, Block block)
    {
        _blockVerts[0] = new VertexPositionColor(block.Position, Color.DeepPink);
        _blockVerts[1] = new VertexPositionColor(block.Position + Vector3.Up * block.Density * 2, Color.DeepPink);
        foreach (var pass in _blockFx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.LineList, _blockVerts, 0, 1);
        }
    }
}
