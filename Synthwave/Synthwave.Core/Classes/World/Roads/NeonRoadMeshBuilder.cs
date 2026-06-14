using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Roads;

public class NeonRoadMeshBuilder(GraphicsDevice device, TerrainSystem terrain)
{
    private readonly GraphicsDevice _device = device;
    private readonly TerrainSystem _terrain = terrain;

    private const float RoadHalfWidth = 8f;
    private const float YOffset = 0.25f;
    private const int Steps = 120;

    public void Build(WorldChunk chunk, List<Spline> roads)
    {
        var verts = new List<RoadVertex>();
        var indices = new List<int>();

        foreach (var road in roads)
            BuildRoad(road, verts, indices);

        chunk.RoadVB = new VertexBuffer(_device, typeof(RoadVertex), verts.Count, BufferUsage.WriteOnly);
        chunk.RoadVB.SetData(verts.ToArray());

        chunk.RoadIB = new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
        chunk.RoadIB.SetData(indices.ToArray());
    }

    private void BuildRoad(Spline road, List<RoadVertex> verts, List<int> idx)
    {
        int baseIndex = verts.Count;

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;

            Vector3 p = road.Evaluate(t);
            p.Y = _terrain.GetHeight(p.X, p.Z) + YOffset;

            Vector3 ahead = road.Evaluate(MathHelper.Clamp(t + 0.01f, 0, 1));
            ahead.Y = p.Y;

            Vector3 forward = Vector3.Normalize(ahead - p);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));

            Vector3 left = p - right * RoadHalfWidth;
            Vector3 rightP = p + right * RoadHalfWidth;

            left.Y = _terrain.GetHeight(left.X, left.Z) + YOffset;
            rightP.Y = _terrain.GetHeight(rightP.X, rightP.Z) + YOffset;

            Vector3 normal = Vector3.Up;

            // UV across road: 0 = left kerb, 0.5 = center, 1 = right kerb
            verts.Add(new RoadVertex(left, normal, new Vector2(0f, t)));
            verts.Add(new RoadVertex(p, normal, new Vector2(0.5f, t)));
            verts.Add(new RoadVertex(rightP, normal, new Vector2(1f, t)));

            if (i > 0)
            {
                int b = baseIndex + (i - 1) * 3;
                int c = baseIndex + i * 3;

                // strip triangles
                idx.Add(b); idx.Add(c); idx.Add(b + 1);
                idx.Add(b + 1); idx.Add(c); idx.Add(c + 1);

                idx.Add(b + 1); idx.Add(c + 1); idx.Add(b + 2);
                idx.Add(b + 2); idx.Add(c + 1); idx.Add(c + 2);
            }
        }
    }
}