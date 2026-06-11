using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Roads;

public class RoadMeshBuilder(GraphicsDevice device, TerrainSystem terrain)
{
    #region Properties
    private readonly GraphicsDevice _device = device;
    private readonly TerrainSystem _terrain = terrain;

    private const float RoadHalfWidth = 8f;
    private const float RoadYOffset = 0.25f;
    private const int Steps = 120;  // tessellation per road
    #endregion

    #region Methods
    public void Build(WorldChunk chunk, List<Spline> roads)
    {
        // Always mark road as attempted so we never loop forever
        if (roads.Count == 0) return;

        var vertices = new List<VertexPositionColor>();
        var indices = new List<int>();

        foreach (var road in roads)
            AppendRoad(road, vertices, indices);

        if (vertices.Count == 0) return;

        chunk.RoadVB = new VertexBuffer(_device,
            typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
        chunk.RoadVB.SetData(vertices.ToArray());

        chunk.RoadIB = new IndexBuffer(_device,
            IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
        chunk.RoadIB.SetData(indices.ToArray());
    }

    private void AppendRoad(Spline road, List<VertexPositionColor> verts, List<int> idx)
    {
        int baseVert = verts.Count;

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;

            Vector3 p = road.Evaluate(t);
            // Project to terrain then lift slightly above it
            p.Y = _terrain.GetHeight(p.X, p.Z) + RoadYOffset;

            // Tangent for road direction
            float t2 = MathHelper.Clamp(t + 1f / Steps, 0f, 1f);
            Vector3 ahead = road.Evaluate(t2);
            ahead.Y = 0; p.Y = _terrain.GetHeight(p.X, p.Z) + RoadYOffset;

            Vector3 fwd = ahead - p;
            fwd.Y = 0f;
            if (fwd.LengthSquared() < 1e-5f) fwd = Vector3.UnitZ;
            fwd.Normalize();

            Vector3 right = Vector3.Cross(fwd, Vector3.Up);
            right.Normalize();

            Vector3 lp = p - right * RoadHalfWidth;
            Vector3 rp = p + right * RoadHalfWidth;

            // Snap edge points to terrain height too
            lp.Y = _terrain.GetHeight(lp.X, lp.Z) + RoadYOffset;
            rp.Y = _terrain.GetHeight(rp.X, rp.Z) + RoadYOffset;

            // 3 verts per ring: left edge, centre, right edge
            Color edgeColor = new Color(255, 30, 180);
            Color centreColor = new Color(160, 0, 110);

            verts.Add(new VertexPositionColor(lp, edgeColor));
            verts.Add(new VertexPositionColor(p, centreColor));
            verts.Add(new VertexPositionColor(rp, edgeColor));

            // Stitch triangles between ring i-1 and ring i
            if (i > 0)
            {
                int b = baseVert + (i - 1) * 3;
                int c = baseVert + i * 3;

                // Left half
                idx.Add(b); idx.Add(c); idx.Add(b + 1);
                idx.Add(b + 1); idx.Add(c); idx.Add(c + 1);
                // Right half
                idx.Add(b + 1); idx.Add(c + 1); idx.Add(b + 2);
                idx.Add(b + 2); idx.Add(c + 1); idx.Add(c + 2);
            }
        }
    }
    #endregion
}
