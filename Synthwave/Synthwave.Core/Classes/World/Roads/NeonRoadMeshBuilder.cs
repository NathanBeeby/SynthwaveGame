
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Roads;

public class NeonRoadMeshBuilder
{
    private readonly GraphicsDevice _device;
    private readonly TerrainSystem _terrain;

    // Paved lane area, each side of the centreline.
    private const float RoadHalfWidth = 24f;

    // Squared kerb dimensions — raised box profile.
    private const float KerbWidth = 3f;
    private const float KerbHeight = 0.4f;

    private const float YOffset = 0.05f; // lift slightly above terrain to avoid z-fighting
    private const int Steps = 120;

    public NeonRoadMeshBuilder(GraphicsDevice device, TerrainSystem terrain)
    {
        _device = device;
        _terrain = terrain;
    }

    public void Build(WorldChunk chunk, RoadSplineSystem roadSystem)
    {
        var verts = new List<RoadVertex>();
        var indices = new List<int>();

        foreach (var road in roadSystem.Splines)
        {
            bool isRoundabout = roadSystem.Roundabouts.Contains(road);
            BuildRoad(road, isRoundabout, verts, indices);
        }

        if (verts.Count == 0) return;

        chunk.RoadVB = new VertexBuffer(_device, typeof(RoadVertex), verts.Count, BufferUsage.WriteOnly);
        chunk.RoadVB.SetData(verts.ToArray());

        chunk.RoadIB = new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
        chunk.RoadIB.SetData(indices.ToArray());
    }

    private void BuildRoad(Spline road, bool isRoundabout, List<RoadVertex> verts, List<int> idx)
    {
        int prev = -1;
        float distance = 0f;
        Vector3 prevCentre = road.Evaluate(0f);

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;

            // Evaluate spline and snap to terrain height
            Vector3 p = road.Evaluate(t);
            float h = _terrain.GetHeight(p.X, p.Z) + YOffset;
            p.Y = h;

            if (i > 0)
                distance += Vector3.Distance(prevCentre, p);

            prevCentre = p;

            // Compute tangent & right vector
            float tAhead = MathHelper.Clamp(t + 0.01f, 0f, 1f);
            Vector3 ahead = road.Evaluate(tAhead);
            ahead.Y = _terrain.GetHeight(ahead.X, ahead.Z) + YOffset;

            Vector3 forward = ahead - p;
            if (forward.LengthSquared() < 0.0001f)
                forward = Vector3.Forward;
            forward = Vector3.Normalize(forward);
            Vector3 right = Vector3.Normalize(Vector3.Cross(forward, Vector3.Up));

            int ring = isRoundabout
                ? EmitFlatRing(verts, p, right, h, distance)
                : EmitKerbedRing(verts, p, right, h, distance);

            if (prev >= 0)
            {
                int cols = isRoundabout ? 2 : 8;
                for (int k = 0; k < cols - 1; k++)
                {
                    idx.Add(prev + k);
                    idx.Add(ring + k);
                    idx.Add(prev + k + 1);

                    idx.Add(prev + k + 1);
                    idx.Add(ring + k);
                    idx.Add(ring + k + 1);
                }
            }

            prev = ring;
        }
    }

    // Flat ribbon for roundabouts
    private static int EmitFlatRing(List<RoadVertex> verts, Vector3 p, Vector3 right, float h, float distance)
    {
        int ring = verts.Count;

        Vector3 left = p - right * RoadHalfWidth;
        Vector3 rightPos = p + right * RoadHalfWidth;

        // Absolute Y (h) for both
        left.Y = rightPos.Y = h;

        // UV.x: left=0, right=1; UV.y = distance along spline
        verts.Add(new RoadVertex(left, Vector3.Up, new Vector2(0f, distance), 1f));
        verts.Add(new RoadVertex(rightPos, Vector3.Up, new Vector2(1f, distance), 1f));

        return ring;
    }

    // 8-point kerbed cross-section
    private static int EmitKerbedRing(List<RoadVertex> verts, Vector3 p, Vector3 right, float h, float distance)
    {
        int ring = verts.Count;

        float wOuter = RoadHalfWidth + KerbWidth; // 27f
        float wInner = RoadHalfWidth;             // 24f
        float hTop = h + KerbHeight;

        Vector3[] pos =
        {
            p - right * wOuter, // 0: left outer bottom (kerb foot)
            p - right * wOuter, // 1: left outer top    (top of kerb wall)
            p - right * wInner, // 2: left inner top    (road surface at kerb edge)
            p - right * wInner, // 3: left inner bottom (road surface)
            p + right * wInner, // 4: right inner bottom
            p + right * wInner, // 5: right inner top
            p + right * wOuter, // 6: right outer top
            p + right * wOuter, // 7: right outer bottom
        };

        float[] y = { h, hTop, hTop, h, h, hTop, hTop, h };
        // UV.x: left side (0→0.08), paved center (0.08→0.92), right side (0.92→1)
        float[] u = { 0f, 0f, 0.08f, 0.08f, 0.92f, 0.92f, 1f, 1f };

        // Correct normals: outer faces point *outward*, inner/road faces up
        Vector3[] normal =
        {
            -right, -right, Vector3.Up, Vector3.Up,
            Vector3.Up, Vector3.Up, right, right
        };

        for (int k = 0; k < pos.Length; k++)
        {
            // Ensure *absolute* height — this was the main bug!
            pos[k].Y = y[k];

            verts.Add(new RoadVertex(pos[k], normal[k], new Vector2(u[k], distance), 0f));
        }

        return ring;
    }
}
