using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core._3DModel;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.Renderer;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Sky;
using Synthwave.Core.Classes.World.Terrain;
using Synthwave.Core.Classes.World.Weather;
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
    private BasicEffect _roadFx;
    private BasicEffect _carFx;
    private BasicEffect _blockFx;
    private BasicEffect _centreLineFx;
    private BasicEffect _walkwayFx;

    private VertexPositionTexture[] _fullscreenQuad;

    // Render targets — allocated once, never per-frame
    private RenderTarget2D _mainRT;
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


    private SpriteBatch _tyreBatch;

    private readonly VertexPositionColor[] _carVerts = new VertexPositionColor[2];
    private readonly VertexPositionColor[] _blockVerts = new VertexPositionColor[2];

    private const float RoadHalfWidth = 10f;
    private const float WalkwayWidth = 4f;
    private const float CentreLineHWidth = 0.3f;

    private float _dashTimer;

    private RenderTarget2D _sceneRT;

    private GraphicsDevice _device;
    private SpriteBatch _spriteBatch;

    private bool _disablePostFX = false; // IMPORTANT DEBUG SWITCH
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
        _roadFx = MakeEffect(device);
        _carFx = MakeEffect(device);
        _blockFx = MakeEffect(device);
        _centreLineFx = MakeEffect(device);
        _walkwayFx = MakeEffect(device);
    }

    private void InitializeRenderTargets(GraphicsDevice device)
    {
        int w = device.PresentationParameters.BackBufferWidth;
        int h = device.PresentationParameters.BackBufferHeight;
        // _mainRT line removed — _sceneRT now carries the depth buffer
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
        InitializeWorld(device, camera);
        mFactory = new MaterialFactory(content, device);
        _device = device;
        _spriteBatch = new SpriteBatch(device);

        _sceneRT = new RenderTarget2D(device,device.Viewport.Width,device.Viewport.Height,false,SurfaceFormat.Color,DepthFormat.Depth24);

        Models = new ModelLoader(content, device, mFactory);
        Models.LoadModels();
        Models.Populate(Roads, Terrain);
        InitializeEffects(device);
        InitializeRenderTargets(device);

        // Clear tyre-mark surface once
        device.SetRenderTarget(_tyreMarkRT);
        device.Clear(Color.Transparent);
        device.SetRenderTarget(null);

        InitializeQuads();
        InitializeEffects(content);
        TextureStore.Initialize(device);
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
        _dashTimer += dt;

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
                current += weather.CurrentWeather is WeatherType.Rain or WeatherType.HeavyRain
                    ? dt * 0.05f : 0f;
                current = MathHelper.Clamp(current * 0.99f, 0f, 1f);
                Terrain.SetPuddle(x, z, current);
            }
    }
    public void Draw(Camera3D camera, WeatherSystem weather, GameTime gameTime)
    {
        DrawSceneToRT(camera, weather, gameTime);
        ApplyPostFX(camera, weather);
    }
    private static BasicEffect MakeEffect(GraphicsDevice device) =>
        new(device) { VertexColorEnabled = true, LightingEnabled = false };
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

    private void DrawRoads(
        GraphicsDevice device, Camera3D camera,
        List<WorldChunk> chunks, WeatherSystem weather)
    {
        _roadFx.View = camera.View;
        _roadFx.Projection = camera.Projection;
        _roadFx.World = Matrix.Identity;

        device.BlendState = BlendState.Opaque;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.RoadVB == null) continue;
            device.SetVertexBuffer(chunk.RoadVB);
            device.Indices = chunk.RoadIB;
            foreach (var pass in _roadFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList, 0, 0,
                    chunk.RoadIB.IndexCount / 3);
            }
        }

        bool dashOn = (int)(_dashTimer / 0.5f) % 2 == 0;
        if (dashOn)
        {
            _centreLineFx.View = camera.View;
            _centreLineFx.Projection = camera.Projection;
            _centreLineFx.World = Matrix.Identity;

            var centreVerts = new List<VertexPositionColor>();
            foreach (var road in Roads.Splines)
                BuildCentreDash(road, centreVerts);

            FlushVerts(device, _centreLineFx, centreVerts, BlendState.Additive);
        }
    }

    private void BuildCentreDash(Spline road, List<VertexPositionColor> verts)
    {
        const float tStep = 0.005f;
        for (float t = 0; t < 1f - tStep; t += tStep)
        {
            Vector3 p0 = Terrain.ProjectToTerrain(road.Evaluate(t));
            Vector3 p1 = Terrain.ProjectToTerrain(road.Evaluate(t + tStep));
            Vector3 tangent = Vector3.Normalize(p1 - p0);
            Vector3 right = Vector3.Normalize(Vector3.Cross(tangent, Vector3.Up));
            float yOff = 0.06f;

            EmitQuad(verts,
                p0 - right * CentreLineHWidth + Vector3.Up * yOff,
                p0 + right * CentreLineHWidth + Vector3.Up * yOff,
                p1 - right * CentreLineHWidth + Vector3.Up * yOff,
                p1 + right * CentreLineHWidth + Vector3.Up * yOff,
                Color.Cyan);
        }
    }

    private static void FlushVerts(GraphicsDevice device, BasicEffect fx, List<VertexPositionColor> verts, BlendState blend)
    {
        if (verts.Count < 3) return;
        device.BlendState = blend;
        foreach (var pass in fx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleList,
                verts.ToArray(), 0, verts.Count / 3);
        }
        device.BlendState = BlendState.Opaque;
    }

    private static void EmitQuad(List<VertexPositionColor> list, Vector3 bl, Vector3 br, Vector3 tl, Vector3 tr, Color color)
    {
        list.Add(new VertexPositionColor(bl, color));
        list.Add(new VertexPositionColor(br, color));
        list.Add(new VertexPositionColor(tl, color));
        list.Add(new VertexPositionColor(tl, color));
        list.Add(new VertexPositionColor(br, color));
        list.Add(new VertexPositionColor(tr, color));
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
        DrawRoads(device, camera, chunks, weather);
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

    private static void SetParam(Effect effect, string name, Vector2 value) => effect.Parameters[name]?.SetValue(value);
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
    private void DrawSceneToRT(Camera3D camera, WeatherSystem weather, GameTime gameTime)
    {
        _device.SetRenderTarget(_sceneRT);

        _device.Clear(new Color(10, 0, 30));

        _device.DepthStencilState = DepthStencilState.Default;
        _device.BlendState = BlendState.Opaque;
        _device.RasterizerState = RasterizerState.CullCounterClockwise;

        Sky.DrawSky(_device, camera);
        Ground.Draw(camera, gameTime);

        foreach (var car in Traffic.Cars) DrawCarGPU(_device, car);
        foreach (var block in City.Blocks) DrawBlockGPU(_device, block);

        _device.SetRenderTarget(null);
    }

    private void ApplyPostFX(Camera3D camera, WeatherSystem weather)
    {
        ApplyEffect(_sceneRT, _postRT, _rainEffect);
    }

    private void ApplyEffect(RenderTarget2D input, RenderTarget2D output, Effect effect)
    {
        _device.SetRenderTarget(output);
        _device.Clear(Color.Transparent);

        _spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, null, null, effect);

        effect.Parameters["SceneTex"]?.SetValue(input);

        _spriteBatch.Draw(input, new Rectangle(0, 0, _device.Viewport.Width, _device.Viewport.Height), Color.White);

        _spriteBatch.End();

        _device.SetRenderTarget(null);
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