using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Sky;

public class Stars
{
    #region Properties
    private VertexPositionColor[] _verts;
    private BasicEffect _fx;
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
        var rng = new Random(42);
        var verts = new List<VertexPositionColor>(1800);

        for (int i = 0; i < 1800; i++)
        {
            // Spread over the full screen NDC space, pushed to far Z=0.999
            float x = (float)(rng.NextDouble() * 2.0 - 1.0);
            float y = (float)(rng.NextDouble() * 1.0);        // top half of sky only
            float brightness = 0.4f + (float)rng.NextDouble() * 0.6f;

            // A few coloured stars for synthwave flair
            Color c;
            int roll = rng.Next(10);
            if (roll == 0) c = new Color(brightness, brightness * 0.5f, 1f);  // blue
            else if (roll == 1) c = new Color(1f, brightness * 0.5f, brightness);  // pink
            else c = new Color(brightness, brightness, brightness);  // white

            verts.Add(new VertexPositionColor(new Vector3(x, y, 0.999f), c));
        }

        _verts = [.. verts];
    }

    public void Draw(GraphicsDevice device, float visibility)
    {
        if (visibility < 0.01f) return;

        // Modulate alpha via colour (no alpha blending needed for stars)
        // We tint them towards black when sun is up
        var tinted = new VertexPositionColor[_verts.Length];
        for (int i = 0; i < _verts.Length; i++)
        {
            Color c = _verts[i].Color;
            tinted[i] = new VertexPositionColor(
                _verts[i].Position,
                new Color(
                    (byte)(c.R * visibility),
                    (byte)(c.G * visibility),
                    (byte)(c.B * visibility)));
        }

        var prev = device.DepthStencilState;
        device.DepthStencilState = DepthStencilState.None;

        foreach (var pass in _fx.CurrentTechnique.Passes)
        {
            pass.Apply();
            device.DrawUserPrimitives(PrimitiveType.PointList, tinted, 0, tinted.Length);
        }

        device.DepthStencilState = prev;
    }
    #endregion
}


