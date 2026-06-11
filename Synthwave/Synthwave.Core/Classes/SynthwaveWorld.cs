using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.Renderer;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Sky;
using Synthwave.Core.Classes.World.Terrain;
using System;
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

    private BasicEffect _terrainFx;
    private BasicEffect _roadFx;
    private BasicEffect _carFx;
    private BasicEffect _blockFx;
    private BasicEffect _fsqEffect;

    private VertexPositionTexture[] _fullscreenQuad;

    RenderTarget2D mainRT;
    RenderTarget2D neonRT;
    Effect neonMaskEffect;
    Effect postProcessEffect;

    private readonly VertexPositionColor[] _carVerts = new VertexPositionColor[2];
    private readonly VertexPositionColor[] _blockVerts = new VertexPositionColor[2];
    #endregion

    #region Methods
    public void Initialize(ContentManager content, GraphicsDevice device, Camera3D camera)
    {
        Terrain = new TerrainSystem();
        Roads = new RoadSplineSystem();
        City = new CityBlockGenerator();
        Traffic = new TrafficSystem();
        Lighting = new LightingSystem();
        Sky = new SkySystem();

        Sky.Initialize(device);  // Stars + Sun need the device

        Bloom = new BloomRenderer { Camera = camera };
        Bloom.Initialize(device);

        Ground = new SynthwaveGroundRenderer();
        Ground.Initialize(device);

        Infinite = new InfiniteWorldManager(device, Terrain);

        Roads.Generate();
        City.Generate(Roads, Terrain);
        Traffic.Spawn(Roads, Terrain);

        _terrainFx = MakeEffect(device);
        _roadFx = MakeEffect(device);
        _carFx = MakeEffect(device);
        _blockFx = MakeEffect(device);

        mainRT = new RenderTarget2D(device,device.PresentationParameters.BackBufferWidth,device.PresentationParameters.BackBufferHeight);

        neonRT = new RenderTarget2D(device,device.PresentationParameters.BackBufferWidth,device.PresentationParameters.BackBufferHeight);

        _fsqEffect = new BasicEffect(device)
        {
            TextureEnabled = true,
            VertexColorEnabled = false,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.Identity
        }; 

        _fullscreenQuad =
        [
    // X,Y,Z       U,V
    new VertexPositionTexture(new Vector3(-1,  1, 0), new Vector2(0, 0)),
    new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),
    new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),

    new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
    new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0)),
    new VertexPositionTexture(new Vector3( 1, -1, 0), new Vector2(1, 1)),
];

        neonMaskEffect = content.Load<Effect>("Shaders/NeonMask");
        postProcessEffect = content.Load<Effect>("Shaders/PostProcessingShader");
    }

    private static BasicEffect MakeEffect(GraphicsDevice device) => new(device) { VertexColorEnabled = true, LightingEnabled = false };

    public void Update(GameTime time, Camera3D camera, InputHandler input)
    {
        float dt = (float)time.ElapsedGameTime.TotalSeconds;

#if DEBUG
        if (Debugger.IsAttached)
        {
            // Fast-forward / rewind time
            if (input.IsKeyDown(Keys.PageUp)) Sky.SkipTime(dt * 2f);
            if (input.IsKeyDown(Keys.PageDown)) Sky.SkipTime(-dt * 2f);

            // Speed tweaks
            if (input.WasJustPressed(Keys.OemPlus) || input.WasJustPressed(Keys.Add))
                camera.Vehicle.CurrentSpeed = MathHelper.Min(camera.Vehicle.CurrentSpeed + 50f, 2000f);
            if (input.WasJustPressed(Keys.OemMinus) || input.WasJustPressed(Keys.Subtract))
                camera.Vehicle.CurrentSpeed = MathHelper.Max(camera.Vehicle.CurrentSpeed - 50f, 30f);
        }
#endif

        // Terrain-snap only when not flying
        float groundY = Terrain.GetHeight(camera.Position.X, camera.Position.Z);
        camera.SnapToTerrain(groundY);

        Infinite.Update(camera.Position, Roads);
        Traffic.Update(dt);
        Sky.Update(dt);

        Bloom.CameraPosition = camera.Position;
        Lighting.Update(Sky);
        Lighting.Apply(Bloom);
    }
    /*
    public void Draw(GraphicsDevice device, Camera3D camera, GameTime gameTime)
    {
        Sky.DrawSky(device, camera);

        device.DepthStencilState = DepthStencilState.Default;
        device.RasterizerState = RasterizerState.CullNone;
        device.BlendState = BlendState.Opaque;

        Ground.Draw(camera, Terrain, gameTime);

        var chunks = Infinite.GetVisibleChunks(camera.Position);

        _terrainFx.View = camera.View;
        _terrainFx.Projection = camera.Projection;
        _terrainFx.World = Matrix.Identity;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.TerrainVB == null) continue;

            device.SetVertexBuffer(chunk.TerrainVB);
            device.Indices = chunk.TerrainIB;

            foreach (var pass in _terrainFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                // TerrainIB is a LineList index buffer
                device.DrawIndexedPrimitives(PrimitiveType.LineList,baseVertex: 0,startIndex: 0,primitiveCount: chunk.TerrainIB.IndexCount / 2);
            }
        }

        _roadFx.View = camera.View;
        _roadFx.Projection = camera.Projection;
        _roadFx.World = Matrix.Identity;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.RoadVB == null) continue;

            device.SetVertexBuffer(chunk.RoadVB);
            device.Indices = chunk.RoadIB;

            foreach (var pass in _roadFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.TriangleList,baseVertex: 0,startIndex: 0,primitiveCount: chunk.RoadIB.IndexCount / 3);
            }
        }

        // 5. Traffic cars
        _carFx.View = camera.View;
        _carFx.Projection = camera.Projection;
        _carFx.World = Matrix.Identity;

        foreach (var car in Traffic.Cars)
            DrawCarGPU(device, car);

        // 6. City blocks
        _blockFx.View = camera.View;
        _blockFx.Projection = camera.Projection;
        _blockFx.World = Matrix.Identity;

        foreach (var block in City.Blocks)
            DrawBlockGPU(device, block);
    }
    */
    private void DrawRoads(GraphicsDevice device, Camera3D camera, List<WorldChunk> chunks)
    {
        _roadFx.View = camera.View;
        _roadFx.Projection = camera.Projection;
        _roadFx.World = Matrix.Identity;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.RoadVB == null) continue;

            device.SetVertexBuffer(chunk.RoadVB);
            device.Indices = chunk.RoadIB;

            foreach (var pass in _roadFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(
                    PrimitiveType.TriangleList,
                    0, 0,
                    chunk.RoadIB.IndexCount / 3
                );
            }
        }
    }

    public void Draw(GraphicsDevice device, Camera3D camera, GameTime gameTime)
    {
        device.SetRenderTarget(mainRT);
        device.Clear(Color.Black);

        Sky.DrawSky(device, camera);

        device.DepthStencilState = DepthStencilState.Default;
        device.RasterizerState = RasterizerState.CullNone;
        device.BlendState = BlendState.Opaque;

        Ground.Draw(camera, gameTime);

        var chunks = Infinite.GetVisibleChunks(camera.Position);

        DrawTerrain(device, camera, chunks);
        DrawRoads(device, camera, chunks);

        foreach (var car in Traffic.Cars)
            DrawCarGPU(device, car);

        foreach (var block in City.Blocks)
            DrawBlockGPU(device, block);

        device.SetRenderTarget(neonRT);
        device.Clear(Color.Black);

        Ground.DrawLinesWithEffect(camera, neonMaskEffect);
        device.SetRenderTarget(null);
        device.DepthStencilState = DepthStencilState.None;
        device.RasterizerState = RasterizerState.CullNone;
        device.BlendState = BlendState.Opaque;

        // bind textures
        postProcessEffect.Parameters["SceneTex"].SetValue(mainRT);
        postProcessEffect.Parameters["BloomTex"].SetValue(neonRT);

        foreach (var pass in postProcessEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleList,_fullscreenQuad,0,2);
        }
    }

    private void DrawTerrain(GraphicsDevice device, Camera3D camera, List<WorldChunk> chunks)
    {
        _terrainFx.View = camera.View;
        _terrainFx.Projection = camera.Projection;
        _terrainFx.World = Matrix.Identity;

        foreach (var chunk in chunks)
        {
            if (!chunk.IsBuilt || chunk.TerrainVB == null) continue;

            device.SetVertexBuffer(chunk.TerrainVB);
            device.Indices = chunk.TerrainIB;

            foreach (var pass in _terrainFx.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawIndexedPrimitives(PrimitiveType.LineList,0, 0,chunk.TerrainIB.IndexCount / 2);
            }
        }
    }

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

    #endregion
}

/*
 CREATE HLSL SHADER PROPER:

 true emissive bloom mask
screen-space blur glow
color grading per biome
heat distortion on sand
water shimmer for sea biome
 
 */