using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Terrain;
public class SynthwaveGroundRenderer
{
    #region Properties
    private GraphicsDevice _device;
    private BasicEffect _effect;
    private VertexPositionColor[] _grid;

    // ── Grid config ───────────────────────────────────────────────────────
    private const int HalfSize = 800;   // world units from centre in each direction
    private const int Step = 50;    // distance between lines
    private const float GridY = 0f;
    #endregion

    #region Methods
    public void Initialize(GraphicsDevice device)
    {
        _device = device;
        _effect = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false
        };
        BuildGrid();
    }

    private void BuildGrid()
    {
        var verts = new List<VertexPositionColor>();

        Color dim = new Color(80, 0, 140);   // base neon purple
        Color bright = new Color(200, 20, 255);   // accent line every N steps

        int lineIdx = 0;

        // Lines parallel to Z (vertical when looking forward)
        for (int x = -HalfSize; x <= HalfSize; x += Step)
        {
            Color c = (lineIdx % 5 == 0) ? bright : dim;
            verts.Add(new VertexPositionColor(new Vector3(x, GridY, -HalfSize), c));
            verts.Add(new VertexPositionColor(new Vector3(x, GridY, HalfSize), c));
            lineIdx++;
        }

        lineIdx = 0;

        // Lines parallel to X (horizontal / receding to horizon)
        for (int z = -HalfSize; z <= HalfSize; z += Step)
        {
            Color c = (lineIdx % 5 == 0) ? bright : dim;
            verts.Add(new VertexPositionColor(new Vector3(-HalfSize, GridY, z), c));
            verts.Add(new VertexPositionColor(new Vector3(HalfSize, GridY, z), c));
            lineIdx++;
        }

        _grid = verts.ToArray();
    }

    public void Draw(Camera3D camera)
    {
        if (_grid == null || _grid.Length < 2) return;

        // Snap to grid so the lines appear to scroll without gaps
        float snapX = MathF.Round(camera.Position.X / Step) * Step;
        float snapZ = MathF.Round(camera.Position.Z / Step) * Step;

        _effect.World = Matrix.CreateTranslation(snapX, 0f, snapZ);
        _effect.View = camera.View;
        _effect.Projection = camera.Projection;

        // DepthRead: grid draws on the ground plane but terrain mesh sits on top
        var prevDepth = _device.DepthStencilState;
        _device.DepthStencilState = DepthStencilState.DepthRead;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(
                PrimitiveType.LineList,
                _grid, 0,
                _grid.Length / 2);
        }

        _device.DepthStencilState = prevDepth;
    }
    #endregion
}