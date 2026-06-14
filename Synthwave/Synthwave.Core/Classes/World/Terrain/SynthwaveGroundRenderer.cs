using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World.Biomes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthwave.Core.Classes.World.Terrain;

public class SynthwaveGroundRenderer
{
    #region Properties
    private GraphicsDevice _device;

    private BasicEffect _solidEffect;
    private BasicEffect _lineEffect;

    private VertexPositionColor[] _floorVerts;
    private VertexPositionColor[] _gridLines;

    private const int HalfSize = 800;
    private const int Step = 50;
    private const float GridY = 0f;

    private float _time;
    #endregion

    #region Methods
    public void Initialize(GraphicsDevice device)
    {
        _device = device;

        _solidEffect = new BasicEffect(device)
        {
            VertexColorEnabled = true
        };

        _lineEffect = new BasicEffect(device)
        {
            VertexColorEnabled = true
        };

        BuildFloor();
        BuildGridLines();
    }

    #region Build
    private void BuildFloor()
    {
        var verts = new List<VertexPositionColor>();

        for (int x = -HalfSize; x < HalfSize; x += Step)
        {
            for (int z = -HalfSize; z < HalfSize; z += Step)
            {
                Vector3 v0 = new(x, GridY, z);
                Vector3 v1 = new(x + Step, GridY, z);
                Vector3 v2 = new(x, GridY, z + Step);
                Vector3 v3 = new(x + Step, GridY, z + Step);

                Color c = Color.Black;

                verts.Add(new VertexPositionColor(v0, c));
                verts.Add(new VertexPositionColor(v1, c));
                verts.Add(new VertexPositionColor(v2, c));

                verts.Add(new VertexPositionColor(v2, c));
                verts.Add(new VertexPositionColor(v1, c));
                verts.Add(new VertexPositionColor(v3, c));
            }
        }

        _floorVerts = [.. verts];
    }

    private void BuildGridLines()
    {
        var lines = new List<VertexPositionColor>();

        for (int x = -HalfSize; x <= HalfSize; x += Step)
        {
            for (int z = -HalfSize; z < HalfSize; z += Step)
            {
                Vector3 a = new(x, GridY, z);
                Vector3 b = new(x, GridY, z + Step);

                Color c = BiomeSystem.GetBiomeColor(x, z);
                c = new Color(c.ToVector3() * 0.6f); // soften for neon look

                lines.Add(new VertexPositionColor(a, c));
                lines.Add(new VertexPositionColor(b, c));
            }
        }

        for (int z = -HalfSize; z <= HalfSize; z += Step)
        {
            for (int x = -HalfSize; x < HalfSize; x += Step)
            {
                Vector3 a = new(x, GridY, z);
                Vector3 b = new(x + Step, GridY, z);

                Color c = BiomeSystem.GetBiomeColor(x, z);
                c = new Color(c.ToVector3() * 0.6f);

                lines.Add(new VertexPositionColor(a, c));
                lines.Add(new VertexPositionColor(b, c));
            }
        }

        _gridLines = [.. lines];
    }
    #endregion

    #region Draw
    private void DrawFloor(Matrix world, Camera3D camera)
    {
        _solidEffect.World = world;
        _solidEffect.View = camera.View;
        _solidEffect.Projection = camera.Projection;

        _device.DepthStencilState = DepthStencilState.Default;
        _device.BlendState = BlendState.Opaque;

        foreach (var pass in _solidEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(PrimitiveType.TriangleList,_floorVerts,0,_floorVerts.Length / 3);
        }
    }

    private void DrawGrid(Matrix world, Camera3D camera, float pulse)
    {
        var verts = new VertexPositionColor[_gridLines.Length];

        for (int i = 0; i < _gridLines.Length; i++)
        {
            var v = _gridLines[i];

            Vector3 col = v.Color.ToVector3();

            verts[i] = new VertexPositionColor(v.Position, new Color(col * pulse));
        }

        _lineEffect.World = world;
        _lineEffect.View = camera.View;
        _lineEffect.Projection = camera.Projection;

        _device.BlendState = BlendState.Additive;

        foreach (var pass in _lineEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(PrimitiveType.LineList,verts,0,verts.Length / 2);
        }

        _device.BlendState = BlendState.Opaque;
    }

    public void DrawLinesWithEffect(Camera3D camera, Effect effect)
    {
        float pulse = 0.6f + 0.4f * MathF.Sin(Environment.TickCount * 0.003f);
        var arr = _gridLines.Select(v => new VertexPositionColor(v.Position, v.Color * pulse)).ToArray();

        effect.Parameters["World"]?.SetValue(Matrix.Identity);
        effect.Parameters["View"]?.SetValue(camera.View);
        effect.Parameters["Projection"]?.SetValue(camera.Projection);

        _device.BlendState = BlendState.Additive;

        foreach (var pass in effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(PrimitiveType.LineList, arr, 0, arr.Length / 2);
        }

        _device.BlendState = BlendState.Opaque;
    }

    public void Draw(Camera3D camera, GameTime time)
    {
        _time += (float)time.ElapsedGameTime.TotalSeconds;

        float pulse = 0.6f + 0.4f * MathF.Sin(_time * 3f);

        float snapX = MathF.Round(camera.Position.X / Step) * Step;
        float snapZ = MathF.Round(camera.Position.Z / Step) * Step;

        Matrix world = Matrix.CreateTranslation(snapX, 0, snapZ);

        DrawFloor(world, camera);
        DrawGrid(world, camera, pulse);
    }
    #endregion
    #endregion
}
