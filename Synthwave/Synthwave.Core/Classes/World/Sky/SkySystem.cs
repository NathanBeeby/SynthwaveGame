using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;
using System.Diagnostics;

namespace Synthwave.Core.Classes.World.Sky;

public class SkySystem
{
    #region Properties
    public float TimeOfDay;       // 0–1 continuous
    public Vector3 SunDirection;
    public Vector3 MoonDirection;
    public Vector3 SkyColor;
    public Vector3 HorizonColor;
    public Vector3 NightColor;

    public float DaySpeed = 0.008f; // full cycle ≈ 125 s

    private Stars _stars;
    private SynthwaveSun _sun;
    private BasicEffect _skyFx;

    private readonly VertexPositionColor[] _skyVerts = new VertexPositionColor[12];
    #endregion

    #region Methods
    public void Initialize(GraphicsDevice device)
    {
        _stars = new Stars();
        _stars.Initialize(device);

        _sun = new SynthwaveSun();
        _sun.Initialize(device);

        _skyFx = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.Identity
        };

        ComputeCelestialBodies();
        ComputeSkyColors();
    }

    public void Update(float dt)
    {
        TimeOfDay += dt * DaySpeed;
        if (TimeOfDay > 1f) TimeOfDay -= 1f;

        ComputeCelestialBodies();
        ComputeSkyColors();
    }

    public void SkipTime(float delta)
    {
#if DEBUG
        if (!Debugger.IsAttached) return;
        TimeOfDay = (TimeOfDay + delta + 1f) % 1f;
        ComputeCelestialBodies();
        ComputeSkyColors();
#endif
    }

    private void ComputeCelestialBodies()
    {
        float angle = TimeOfDay * MathHelper.TwoPi;
        SunDirection = new Vector3(MathF.Cos(angle), MathF.Sin(angle), 0f);
        MoonDirection = -SunDirection;
    }

    private void ComputeSkyColors()
    {
        float t = TimeOfDay;

        Vector3 night = new(0.04f, 0.00f, 0.18f);
        Vector3 dawn = new(0.80f, 0.15f, 0.50f);
        Vector3 day = new(0.22f, 0.42f, 0.88f);
        Vector3 dusk = new(0.95f, 0.22f, 0.38f);

        if (t < 0.15f) SkyColor = Vector3.Lerp(night, dawn, t / 0.15f);
        else if (t < 0.25f) SkyColor = Vector3.Lerp(dawn, day, (t - 0.15f) / 0.10f);
        else if (t < 0.40f) SkyColor = day;
        else if (t < 0.55f) SkyColor = Vector3.Lerp(day, dusk, (t - 0.40f) / 0.15f);
        else if (t < 0.65f) SkyColor = Vector3.Lerp(dusk, night, (t - 0.55f) / 0.10f);
        else SkyColor = night;

        float sunUp = MathF.Max(0f, SunDirection.Y);
        HorizonColor = Vector3.Lerp(
            new Vector3(0.35f, 0.00f, 0.55f),  // night horizon: deep purple
            new Vector3(1.00f, 0.20f, 0.60f),  // day horizon: neon pink
            sunUp);

        NightColor = night;
    }

    public float GetSunIntensity() => MathF.Max(0f, SunDirection.Y);
    public float GetMoonIntensity() => MathF.Max(0f, MoonDirection.Y);

    public float GetStarVisibility() => MathHelper.Clamp(1f - GetSunIntensity() * 4f, 0f, 1f);

    // TODO: Potentially add camera into DrawSky
    public void DrawSky(GraphicsDevice device, Camera3D camera)
    {
        var prevDepth = device.DepthStencilState;
        var prevRasterizer = device.RasterizerState;
        var prevBlend = device.BlendState;

        device.DepthStencilState = DepthStencilState.None;
        device.RasterizerState = RasterizerState.CullNone;
        device.BlendState = BlendState.Opaque;

        // ── Gradient background ──────────────────────────────────────
        BuildSkyVerts();

        foreach (var pass in _skyFx.CurrentTechnique.Passes)
        {
            pass.Apply();
            // Lower band (ground → horizon)  — 2 tris = verts 0–5
            device.DrawUserPrimitives(PrimitiveType.TriangleList, _skyVerts, 0, 2);
            // Upper band (horizon → zenith)  — 2 tris = verts 6–11
            device.DrawUserPrimitives(PrimitiveType.TriangleList, _skyVerts, 6, 2);
        }

        // ── Stars (additive-ish: just drawn opaque over sky) ──────────
        _stars.Draw(device, GetStarVisibility());

        // ── Sun ──────────────────────────────────────────────────────
        float sunAngle = TimeOfDay * MathHelper.TwoPi;
        _sun.Draw(device, sunAngle, GetSunIntensity());

        device.DepthStencilState = prevDepth;
        device.RasterizerState = prevRasterizer;
        device.BlendState = prevBlend;
    }

    private void BuildSkyVerts()
    {
        Color top = new(SkyColor);
        Color mid = new(HorizonColor);
        Color bot = new(NightColor * 0.5f);  // dark ground fringe

        // Lower band: bottom → horizon  (screen Y: -1 → 0)
        _skyVerts[0] = new VertexPositionColor(new Vector3(-1f, -1f, 1f), bot);
        _skyVerts[1] = new VertexPositionColor(new Vector3(-1f, 0f, 1f), mid);
        _skyVerts[2] = new VertexPositionColor(new Vector3(1f, -1f, 1f), bot);
        _skyVerts[3] = new VertexPositionColor(new Vector3(1f, -1f, 1f), bot);
        _skyVerts[4] = new VertexPositionColor(new Vector3(-1f, 0f, 1f), mid);
        _skyVerts[5] = new VertexPositionColor(new Vector3(1f, 0f, 1f), mid);

        // Upper band: horizon → zenith  (screen Y: 0 → 1)
        _skyVerts[6] = new VertexPositionColor(new Vector3(-1f, 0f, 1f), mid);
        _skyVerts[7] = new VertexPositionColor(new Vector3(-1f, 1f, 1f), top);
        _skyVerts[8] = new VertexPositionColor(new Vector3(1f, 0f, 1f), mid);
        _skyVerts[9] = new VertexPositionColor(new Vector3(1f, 0f, 1f), mid);
        _skyVerts[10] = new VertexPositionColor(new Vector3(-1f, 1f, 1f), top);
        _skyVerts[11] = new VertexPositionColor(new Vector3(1f, 1f, 1f), top);
    }
#endregion
}
