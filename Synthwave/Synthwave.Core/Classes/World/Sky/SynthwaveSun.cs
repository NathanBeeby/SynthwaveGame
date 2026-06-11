using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Sky;

public class SynthwaveSun
{
    #region Properties
    private VertexPositionColor[] _verts;
    private BasicEffect _fx;
    private const int Segments = 72;
    private const float Radius = 0.08f;  // NDC radius (0–1 scale)
    #endregion

    #region Methods
    public void Initialize(GraphicsDevice device)
    {
        _fx = new BasicEffect(device)
        {
            VertexColorEnabled = true,
            LightingEnabled = false,
            World = Matrix.Identity,
            View = Matrix.Identity,
            Projection = Matrix.Identity
        };
        Build();
    }

    private void Build()
    {
        var verts = new List<VertexPositionColor>(Segments * 3);
        Vector3 centre = Vector3.Zero;

        for (int i = 0; i < Segments; i++)
        {
            float a1 = MathHelper.TwoPi * i / Segments;
            float a2 = MathHelper.TwoPi * (i + 1) / Segments;

            Vector3 p1 = new(MathF.Cos(a1) * Radius, MathF.Sin(a1) * Radius, 0f);
            Vector3 p2 = new(MathF.Cos(a2) * Radius, MathF.Sin(a2) * Radius, 0f);

            float t1 = (MathF.Sin(a1) + 1f) * 0.5f;
            float t2 = (MathF.Sin(a2) + 1f) * 0.5f;
            Color c1 = Color.Lerp(new Color(255, 60, 200), new Color(255, 220, 50), t1);
            Color c2 = Color.Lerp(new Color(255, 60, 200), new Color(255, 220, 50), t2);

            verts.Add(new VertexPositionColor(centre, new Color(255, 160, 80)));
            verts.Add(new VertexPositionColor(p1, c1));
            verts.Add(new VertexPositionColor(p2, c2));
        }

        _verts = [.. verts];
    }

    public void Draw(GraphicsDevice device, float sunAngle, float visibility)
    {
        if (visibility < 0.01f) return;

        float screenX = MathF.Cos(sunAngle) * 0.65f;
        float screenY = MathF.Sin(sunAngle) * 0.75f - 0.05f;

        // Clamp so it never goes fully off screen
        screenX = MathHelper.Clamp(screenX, -0.9f, 0.9f);
        screenY = MathHelper.Clamp(screenY, -0.8f, 0.8f);

        var translated = new VertexPositionColor[_verts.Length];
        for (int i = 0; i < _verts.Length; i++)
        {
            Vector3 p = _verts[i].Position + new Vector3(screenX, screenY, 0.998f);
            Color c = _verts[i].Color;

            translated[i] = new VertexPositionColor(p,
                new Color(
                    (byte)(c.R * visibility),
                    (byte)(c.G * visibility),
                    (byte)(c.B * visibility)));
        }

        DrawSunStripes(device, screenX, screenY, visibility);

        var prevDepth = device.DepthStencilState;
        device.DepthStencilState = DepthStencilState.None;

        foreach (var pass in _fx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleList,
                translated, 0, translated.Length / 3);
        }

        device.DepthStencilState = prevDepth;
    }

    private void DrawSunStripes(GraphicsDevice device, float cx, float cy, float vis)
    {
        const int Lines = 8;
        const float LineGap = Radius * 1.6f / Lines;

        var stripeVerts = new List<VertexPositionColor>(Lines * 6);

        for (int i = 0; i < Lines; i++)
        {
            float yOff = -Radius * 0.05f - i * LineGap;   
            float halfW = MathF.Sqrt(MathF.Max(0f, Radius * Radius - yOff * yOff));
            float lineH = LineGap * 0.35f;   

            float x0 = cx - halfW;
            float x1 = cx + halfW;
            float y0 = cy + yOff;
            float y1 = cy + yOff - lineH;

            Color sc = new((byte)0, (byte)0, (byte)0, (byte)(200 * vis));

            stripeVerts.Add(new VertexPositionColor(new Vector3(x0, y0, 0.997f), sc));
            stripeVerts.Add(new VertexPositionColor(new Vector3(x1, y0, 0.997f), sc));
            stripeVerts.Add(new VertexPositionColor(new Vector3(x0, y1, 0.997f), sc));

            stripeVerts.Add(new VertexPositionColor(new Vector3(x0, y1, 0.997f), sc));
            stripeVerts.Add(new VertexPositionColor(new Vector3(x1, y0, 0.997f), sc));
            stripeVerts.Add(new VertexPositionColor(new Vector3(x1, y1, 0.997f), sc));
        }

        if (stripeVerts.Count < 3) return;

        var arr = stripeVerts.ToArray();
        foreach (var pass in _fx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.TriangleList, arr, 0, arr.Length / 3);
        }
    }
    #endregion
}
