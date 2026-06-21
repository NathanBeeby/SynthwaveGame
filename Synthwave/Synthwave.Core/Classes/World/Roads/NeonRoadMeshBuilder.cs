
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

    // Paved lane area, each side of the centreline.
    private const float RoadHalfWidth = 24f;

    // Squared kerb / sidewalk dimensions — raised box profile.
    private const float KerbWidth = 8f;
    private const float KerbHeight = 0.4f;

    private const float YOffset = 0.05f; // lift slightly above terrain to avoid z-fighting
    private const int Steps = 120;

    // ── Centre line (built entirely as geometry — no shader-side dash math) ──
    private const float CentreLineWidth = 1.4f;
    private const float CentreLineYOffset = YOffset + 0.6f; // sit just above the mirror surface
    private const float DashLength = 6.0f;
    private const float DashGap = 4.0f;
    private const int CentreLineSubSteps = Steps * 4; // finer sampling so dash edges land cleanly

    public void Build(WorldChunk chunk, IEnumerable<Spline> roads, HashSet<Spline> roundabouts)
    {
        var roadVerts = new List<RoadVertex>();
        var roadIdx = new List<int>();
        var sidewalkVerts = new List<RoadVertex>();
        var sidewalkIdx = new List<int>();
        var centreVerts = new List<RoadVertex>();
        var centreIdx = new List<int>();

        foreach (var road in roads)
        {
            bool isRoundabout = roundabouts.Contains(road);

            BuildRoadSurface(road, isRoundabout, roadVerts, roadIdx);

            if (!isRoundabout)
            {
                BuildSidewalks(road, sidewalkVerts, sidewalkIdx);
                BuildCentreLine(road, centreVerts, centreIdx);
            }
        }

        CreateBuffers(roadVerts, roadIdx, out chunk.RoadVB, out chunk.RoadIB);
        CreateBuffers(sidewalkVerts, sidewalkIdx, out chunk.SidewalkVB, out chunk.SidewalkIB);
        CreateBuffers(centreVerts, centreIdx, out chunk.CentreLineVB, out chunk.CentreLineIB);
    }

    private void CreateBuffers(List<RoadVertex> verts, List<int> indices, out VertexBuffer vb, out IndexBuffer ib)
    {
        vb = null;
        ib = null;
        if (verts.Count == 0 || indices.Count == 0) return;

        vb = new VertexBuffer(_device, typeof(RoadVertex), verts.Count, BufferUsage.WriteOnly);
        vb.SetData(verts.ToArray());

        ib = new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
        ib.SetData(indices.ToArray());
    }

    private void BuildRoadSurface(Spline road, bool isRoundabout, List<RoadVertex> verts, List<int> idx)
    {
        int prev = -1;
        float distance = 0f;
        Vector3 prevCentre = road.Evaluate(0f);

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;
            Vector3 p = road.Evaluate(t);
            float h = _terrain.GetHeight(p.X, p.Z) + YOffset;
            p.Y = h;

            if (i > 0) distance += Vector2.Distance(new Vector2(prevCentre.X, prevCentre.Z),new Vector2(p.X, p.Z));
            prevCentre = p;

            Vector3 right = ComputeRight(road, t, p);
            int ring =EmitFlatRing(verts,p,right,distance,isRoundabout ? 1 : 0);
            if (prev >= 0) ConnectRing(idx, prev, ring, 2);

            prev = ring;
        }
    }

    private int EmitFlatRing(List<RoadVertex> verts, Vector3 centre, Vector3 right, float distance, float roadType)
    {
        int ring = verts.Count;

        Vector3 left = centre - right * RoadHalfWidth;
        Vector3 rightPos = centre + right * RoadHalfWidth;

        left.Y = centre.Y;
        rightPos.Y = centre.Y;

        verts.Add(new RoadVertex(left, Vector3.Up, new Vector2(0, distance), roadType));
        verts.Add(new RoadVertex(rightPos, Vector3.Up, new Vector2(1, distance), roadType));

        return ring;
    }

    private void BuildSidewalks(Spline road, List<RoadVertex> verts, List<int> idx)
    {
        int prevLeft = -1, prevRight = -1;
        float distance = 0f;
        Vector3 prevCentre = road.Evaluate(0f);

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;

            Vector3 p = road.Evaluate(t);
            float h = _terrain.GetHeight(p.X, p.Z) + YOffset;
            p.Y = h;

            if (i > 0) distance += Vector3.Distance(prevCentre, p);
            prevCentre = p;

            Vector3 right = ComputeRight(road, t, p);

            int leftRing = EmitSidewalkRing(verts, p, right, h, distance, isLeft: true);
            int rightRing = EmitSidewalkRing(verts, p, right, h, distance, isLeft: false);

            if (prevLeft >= 0)
            {
                ConnectRing(idx, prevLeft, leftRing, 4);
                ConnectRing(idx, prevRight, rightRing, 4);
            }

            prevLeft = leftRing;
            prevRight = rightRing;
        }
    }

    private int EmitSidewalkRing(List<RoadVertex> verts, Vector3 p, Vector3 right, float h, float distance, bool isLeft)
    {
        int ring = verts.Count;

        float wOuter = RoadHalfWidth + KerbWidth;
        float wInner = RoadHalfWidth;

        Vector3 outerPos = p + right * (isLeft ? -wOuter : wOuter);
        Vector3 innerPos = p + right * (isLeft ? -wInner : wInner);

        // Same baseline as the road surface — no independent terrain sample.
        outerPos.Y = h;
        innerPos.Y = h;

        Vector3[] pos;
        Vector3[] normal;

        if (isLeft)
        {
            pos = [outerPos, outerPos, innerPos, innerPos];
            normal = [Vector3.Up, Vector3.Up, Vector3.Up, Vector3.Up];
        }
        else
        {
            pos = [innerPos, innerPos, outerPos, outerPos];
            normal = [Vector3.Up, Vector3.Up, right, right];
        }

        float[] heightAdd = [0f, KerbHeight, KerbHeight, 0f];

        for (int k = 0; k < 4; k++)
        {
            Vector3 vp = pos[k];
            vp.Y = h + heightAdd[k];

            float u = (k < 2) ? 0f : KerbWidth;
            verts.Add(new RoadVertex(vp, normal[k], new Vector2(u, distance), 0));
        }

        return ring;
    }

    private void BuildCentreLine(Spline road, List<RoadVertex> verts, List<int> idx)
    {
        float period = DashLength + DashGap;
        float distance = 0f;

        Vector3 prevPoint = road.Evaluate(0f);
        prevPoint.Y = _terrain.GetHeight(prevPoint.X, prevPoint.Z) + CentreLineYOffset;

        for (int i = 1; i <= CentreLineSubSteps; i++)
        {
            float t = i / (float)CentreLineSubSteps;

            Vector3 p = road.Evaluate(t);
            p.Y = _terrain.GetHeight(p.X, p.Z) + CentreLineYOffset;

            float segLen = Vector3.Distance(prevPoint, p);
            float midDistance = distance + segLen * 0.5f;
            distance += segLen;

            bool dashOn = segLen > 0.0001f && (midDistance % period) < DashLength;

            if (dashOn)
            {
                Vector3 forward = Vector3.Normalize(p - prevPoint);
                Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.Up, forward));

                Vector3 a = prevPoint - right * (CentreLineWidth * 0.5f);
                Vector3 b = prevPoint + right * (CentreLineWidth * 0.5f);
                Vector3 c = p - right * (CentreLineWidth * 0.5f);
                Vector3 d = p + right * (CentreLineWidth * 0.5f);

                int baseIdx = verts.Count;
                verts.Add(new RoadVertex(a, Vector3.Up, new Vector2(0f, distance - segLen), 0f));
                verts.Add(new RoadVertex(b, Vector3.Up, new Vector2(1f, distance - segLen), 0f));
                verts.Add(new RoadVertex(c, Vector3.Up, new Vector2(0f, distance), 0f));
                verts.Add(new RoadVertex(d, Vector3.Up, new Vector2(1f, distance), 0f));

                idx.Add(baseIdx + 0); idx.Add(baseIdx + 2); idx.Add(baseIdx + 1);
                idx.Add(baseIdx + 1); idx.Add(baseIdx + 2); idx.Add(baseIdx + 3);
            }

            prevPoint = p;
        }
    }

    private Vector3 ComputeRight(Spline road, float t, Vector3 p)
    {
        const float eps = 0.01f;
        float tA = MathHelper.Clamp(t + eps, 0f, 1f);
        float tB = MathHelper.Clamp(t - eps, 0f, 1f);

        Vector3 ahead = road.Evaluate(tA);
        Vector3 behind = road.Evaluate(tB);

        Vector3 forward = new Vector3(ahead.X - behind.X, 0f, ahead.Z - behind.Z);

        if (forward.LengthSquared() < 0.0001f) forward = Vector3.Forward;

        forward.Normalize();

        return Vector3.Normalize(Vector3.Cross(Vector3.Up, forward));
    }

    private static void ConnectRing(List<int> idx, int prevRing, int ring, int cols)
    {
        for (int k = 0; k < cols - 1; k++)
        {
            idx.Add(prevRing + k);
            idx.Add(ring + k);
            idx.Add(prevRing + k + 1);

            idx.Add(prevRing + k + 1);
            idx.Add(ring + k);
            idx.Add(ring + k + 1);
        }
    }
}