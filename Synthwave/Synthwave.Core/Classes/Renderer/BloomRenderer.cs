using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Renderer;

public class BloomRenderer
{
    #region Properties
    public Camera3D Camera;
    public Vector3 CameraPosition;
    public Vector3 AmbientLight;

    private GraphicsDevice _device;
    private BasicEffect _effect;

    // Line vertex pairs — always even count
    private readonly List<VertexPositionColor> _lineBuffer = [];

    public NeonMaterialSystem Neon = new();
    public FogSystem Fog = new();
    public OutrunScaler Outrun = new();

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
    }

    public void Begin(Camera3D cam)
    {
        Camera = cam;
        CameraPosition = cam.Position;
        _lineBuffer.Clear();
    }

    public void End()
    {
        Flush();
    }
    public void DrawGlowLine(Vector3 a, Vector3 b, Vector3 color)
    {
        _lineBuffer.Add(new VertexPositionColor(a, new Color(color * 2f)));
        _lineBuffer.Add(new VertexPositionColor(b, new Color(color * 2f)));
    }
    public void DrawLine(Vector3 a, Vector3 b, Color color)
    {
        _lineBuffer.Add(new VertexPositionColor(a, color));
        _lineBuffer.Add(new VertexPositionColor(b, color));
    }

    public void DrawRoad(Spline road, TerrainSystem terrain)
    {
        for (float t = 0; t < 1f; t += 0.01f)
        {
            Vector3 p = road.Evaluate(t);
            p = terrain.ProjectToTerrain(p);

            Vector3 p2 = road.Evaluate(MathHelper.Clamp(t + 0.01f, 0f, 1f));
            p2 = terrain.ProjectToTerrain(p2);

            float dist = Vector3.Distance(CameraPosition, p);
            Vector3 color = NeonMaterialSystem.Evaluate(t, 1.5f) + AmbientLight;
            color = Fog.Apply(color, dist);

            DrawLine(p, p2, new Color(color));
        }
    }

    public void DrawCityBlock(Block b)
    {
        Vector3 basePos = b.Position;
        for (int i = 0; i < b.Density; i++)
        {
            Vector3 bot = basePos + new Vector3(0, i * 6, 0);
            Vector3 top = basePos + new Vector3(0, i * 6 + 5, 0);
            DrawLine(bot, top, new Color(1f, 0.2f, 0.8f));
        }
    }

    public void DrawCar(Car c)
    {
        float dist = Vector3.Distance(CameraPosition, c.Position);
        Vector3 color = new Vector3(0.2f, 1f, 0.9f) + AmbientLight;
        color = Fog.Apply(color, dist);

        DrawLine(c.Position, c.Position + Vector3.Up * 2f, new Color(color));
    }

    public void DrawDebugOrigin()
    {
        DrawLine(-Vector3.UnitX * 5f, Vector3.UnitX * 5f, Color.Red);
        DrawLine(-Vector3.UnitY * 5f, Vector3.UnitY * 5f, Color.Green);
        DrawLine(-Vector3.UnitZ * 5f, Vector3.UnitZ * 5f, Color.Blue);
    }

    // ── Internal flush ───────────────────────────────────────────────────

    private void Flush()
    {
        // LineList requires pairs; drop a trailing orphan if somehow present
        int count = _lineBuffer.Count & ~1; // round down to even
        if (count < 2) return;

        _effect.World = Matrix.Identity;
        _effect.View = Camera.View;
        _effect.Projection = Camera.Projection;

        foreach (var pass in _effect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _device.DrawUserPrimitives(PrimitiveType.LineList,_lineBuffer.ToArray(),0,count / 2);
        }

        _lineBuffer.Clear();
    }
    #endregion
}