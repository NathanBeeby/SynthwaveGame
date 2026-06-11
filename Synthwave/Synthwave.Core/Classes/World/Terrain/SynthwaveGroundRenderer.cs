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

                // two triangles per quad
                verts.Add(new VertexPositionColor(v0, Color.Black));
                verts.Add(new VertexPositionColor(v1, Color.Black));
                verts.Add(new VertexPositionColor(v2, Color.Black));

                verts.Add(new VertexPositionColor(v2, Color.Black));
                verts.Add(new VertexPositionColor(v1, Color.Black));
                verts.Add(new VertexPositionColor(v3, Color.Black));
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

                lines.Add(new VertexPositionColor(a, Color.Lime));
                lines.Add(new VertexPositionColor(b, Color.Lime));
            }
        }

        for (int z = -HalfSize; z <= HalfSize; z += Step)
        {
            for (int x = -HalfSize; x < HalfSize; x += Step)
            {
                Vector3 a = new(x, GridY, z);
                Vector3 b = new(x + Step, GridY, z);

                lines.Add(new VertexPositionColor(a, Color.Lime));
                lines.Add(new VertexPositionColor(b, Color.Lime));
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
            verts[i] = new VertexPositionColor(v.Position,new Color(v.Color.ToVector3() * pulse));
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
//public class SynthwaveGroundRenderer
//{
//    #region Properties
//    private GraphicsDevice _device;
//    private BasicEffect _effect;
//    private VertexPositionColor[] _grid;

//    // ── Grid config ───────────────────────────────────────────────────────
//    private const int HalfSize = 800;   // world units from centre in each direction
//    private const int Step = 50;    // distance between lines
//    private const float GridY = 0f;
//    #endregion

//    #region Methods
//    public void Initialize(GraphicsDevice device)
//    {
//        _device = device;
//        _effect = new BasicEffect(device)
//        {
//            VertexColorEnabled = true,
//            LightingEnabled = false
//        };
//        BuildGrid();
//    }

//    private void BuildGrid()
//    {
//        var verts = new List<VertexPositionColor>();

//        Color dim = new(80, 0, 140);   // base neon purple
//        Color bright = new(200, 20, 255);   // accent line every N steps

//        int lineIdx = 0;

//        // Lines parallel to Z (vertical when looking forward)
//        for (int x = -HalfSize; x <= HalfSize; x += Step)
//        {
//            Color c = (lineIdx % 5 == 0) ? bright : dim;
//            verts.Add(new VertexPositionColor(new Vector3(x, GridY, -HalfSize), c));
//            verts.Add(new VertexPositionColor(new Vector3(x, GridY, HalfSize), c));
//            lineIdx++;
//        }

//        lineIdx = 0;

//        // Lines parallel to X (horizontal / receding to horizon)
//        for (int z = -HalfSize; z <= HalfSize; z += Step)
//        {
//            Color c = (lineIdx % 5 == 0) ? bright : dim;
//            verts.Add(new VertexPositionColor(new Vector3(-HalfSize, GridY, z), c));
//            verts.Add(new VertexPositionColor(new Vector3(HalfSize, GridY, z), c));
//            lineIdx++;
//        }

//        _grid = [.. verts];
//    }

//    public void Draw(Camera3D camera)
//    {
//        if (_grid == null || _grid.Length < 2) return;

//        // Snap to grid so the lines appear to scroll without gaps
//        float snapX = MathF.Round(camera.Position.X / Step) * Step;
//        float snapZ = MathF.Round(camera.Position.Z / Step) * Step;

//        _effect.World = Matrix.CreateTranslation(snapX, 0f, snapZ);
//        _effect.View = camera.View;
//        _effect.Projection = camera.Projection;

//        // DepthRead: grid draws on the ground plane but terrain mesh sits on top
//        var prevDepth = _device.DepthStencilState;
//        _device.DepthStencilState = DepthStencilState.DepthRead;

//        foreach (var pass in _effect.CurrentTechnique.Passes)
//        {
//            pass.Apply();
//            _device.DrawUserPrimitives(PrimitiveType.LineList,_grid, 0,_grid.Length / 2);
//        }

//        _device.DepthStencilState = prevDepth;
//    }
//    #endregion
//}