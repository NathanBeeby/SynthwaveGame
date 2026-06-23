
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

    private const float YOffset = 0.45f;
    private const int Steps = 120;

    // ── Centre line (built entirely as geometry — no shader-side dash math) ──
    private const float CentreLineWidth = 1.4f;
    private const float CentreLineYOffset = YOffset + 0.15f; // sit just above the mirror surface
    private const float DashLength = 8.0f;
    private const float DashGap = 5.0f;
    private const int CentreLineSubSteps = Steps * 6; // finer sampling so dash edges land cleanly

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

        System.Diagnostics.Debug.WriteLine($"Road: {chunk.RoadVB?.VertexCount}");

        System.Diagnostics.Debug.WriteLine($"Sidewalk: {chunk.SidewalkVB?.VertexCount}");

        System.Diagnostics.Debug.WriteLine($"Centre: {chunk.CentreLineVB?.VertexCount}");
    }

    private void CreateBuffers(List<RoadVertex> verts, List<int> indices,out VertexBuffer vb, out IndexBuffer ib)
    {
        vb = null;
        ib = null;
        if (verts.Count == 0 || indices.Count == 0) return;

        vb = new VertexBuffer(_device, typeof(RoadVertex), verts.Count, BufferUsage.WriteOnly);
        vb.SetData(verts.ToArray());

        ib = new IndexBuffer(_device, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.WriteOnly);
        ib.SetData(indices.ToArray());
    }

    private void BuildRoadSurface(Spline road, bool isRoundabout,List<RoadVertex> verts, List<int> idx)
    {
        int prevRing = -1;
        float distance = 0f;
        Vector3 prevCentre = road.Evaluate(0f);

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;
            Vector3 p = road.Evaluate(t);
            float h = _terrain.GetHeight(p.X, p.Z) + (YOffset * 2);
            p.Y = h;

            if (i > 0) distance += Vector2.Distance(new Vector2(prevCentre.X, prevCentre.Z),new Vector2(p.X, p.Z));
            prevCentre = p;

            Vector3 right = ComputeRight(road, t);
            // FIX: capture absolute base index before emitting into the shared list
            int ringBase = verts.Count;
            EmitFlatRing(verts, p, right, distance, isRoundabout ? 1f : 0f);

            if (prevRing >= 0)
                ConnectRings(idx, prevRing, ringBase, 2);

            prevRing = ringBase;
        }
    }

    private static void EmitFlatRing(List<RoadVertex> verts,Vector3 centre, Vector3 right,float distance, float roadType)
    {
        Vector3 leftPos = centre - right * RoadHalfWidth;
        Vector3 rightPos = centre + right * RoadHalfWidth;
        const float Crown = 0.08f;

        leftPos.Y = centre.Y - Crown;
        rightPos.Y = centre.Y - Crown;
        centre.Y += Crown;

        verts.Add(new RoadVertex(leftPos, Vector3.Up, new Vector2(0f, distance), roadType));
        verts.Add(new RoadVertex(rightPos, Vector3.Up, new Vector2(1f, distance), roadType));
    }

    private void BuildSidewalks(Spline road, List<RoadVertex> verts, List<int> idx)
    {
        int prevLeft = -1;
        int prevRight = -1;

        float distance = 0f;
        Vector3 prevCentre = road.Evaluate(0f);

        for (int i = 0; i <= Steps; i++)
        {
            float t = i / (float)Steps;
            Vector3 p = road.Evaluate(t);
            float h = _terrain.GetHeight(p.X, p.Z) + YOffset;
            p.Y = h;

            if (i > 0)
                distance += Vector3.Distance(prevCentre, p);

            prevCentre = p;

            Vector3 right = ComputeRight(road, t);

            int leftBase = verts.Count;
            EmitSidewalkRing(verts, p, right, h, distance, isLeft: true);

            int rightBase = verts.Count;
            EmitSidewalkRing(verts, p, right, h, distance, isLeft: false);

            if (prevLeft >= 0)
            {
                ConnectRings(idx, prevLeft, leftBase, 4);
                ConnectRings(idx, prevRight, rightBase, 4);
            }

            prevLeft = leftBase;
            prevRight = rightBase;
        }
    }
    //private void BuildSidewalks(Spline road, List<RoadVertex> verts, List<int> idx)
    //{
    //    int prevLeft = -1, prevRight = -1;
    //    float distance = 0f;
    //    Vector3 prevCentre = road.Evaluate(0f);

    //    for (int i = 0; i <= Steps; i++)
    //    {
    //        float t = i / (float)Steps;
    //        Vector3 p = road.Evaluate(t);
    //        float h = _terrain.GetHeight(p.X, p.Z) + YOffset;
    //        p.Y = h;

    //        if (i > 0)
    //            distance += Vector3.Distance(prevCentre, p);
    //        prevCentre = p;

    //        Vector3 right = ComputeRight(road, t);

    //        int leftBase = verts.Count;
    //        EmitSidewalkRing(verts, p, right, h, distance, isLeft: true);

    //        int rightBase = verts.Count;
    //        EmitSidewalkRing(verts, p, right, h, distance, isLeft: false);

    //        if (prevLeft >= 0)
    //        {
    //            ConnectRings(idx, prevLeft, leftBase, 4);
    //            ConnectRings(idx, prevRight, rightBase, 4);
    //        }

    //        prevLeft = leftBase;
    //        prevRight = rightBase;
    //    }
    //}

    private static void EmitSidewalkRing(List<RoadVertex> verts,Vector3 p, Vector3 right,float h, float distance,bool isLeft)
    {
        float sign = isLeft ? -1f : 1f;
        float wInner = RoadHalfWidth;
        float wOuter = RoadHalfWidth + KerbWidth;

        Vector3 innerPos = p + right * (sign * wInner);
        Vector3 outerPos = p + right * (sign * wOuter);
        innerPos.Y = h;
        outerPos.Y = h;

        Vector3 wallNormal = right * sign;   // points away from road centre
        verts.Add(new RoadVertex(new Vector3(innerPos.X, h, innerPos.Z),wallNormal,new Vector2(0f, distance), 0f));
        verts.Add(new RoadVertex(new Vector3(innerPos.X, h + KerbHeight, innerPos.Z),Vector3.Up,new Vector2(0f, distance), 0f));
        verts.Add(new RoadVertex(new Vector3(outerPos.X, h + KerbHeight, outerPos.Z),Vector3.Up,new Vector2(KerbWidth, distance), 0f));
        verts.Add(new RoadVertex(new Vector3(outerPos.X, h, outerPos.Z),wallNormal,new Vector2(KerbWidth, distance), 0f));
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
            float midDist = distance + segLen * 0.5f;
            distance += segLen;

            if (segLen < 0.0001f)
            {
                prevPoint = p;
                continue;
            }

            bool dashOn = (midDist % period) < DashLength;

            if (dashOn)
            {
                Vector3 forward = Vector3.Normalize(p - prevPoint);
                if (Vector3.Cross(Vector3.Up, forward).LengthSquared() < 0.0001f) forward = Vector3.Forward;

                Vector3 right = Vector3.Normalize(Vector3.Cross(Vector3.Up, forward));
                float hw = CentreLineWidth * 0.5f;

                Vector3 a = prevPoint - right * hw;
                Vector3 b = prevPoint + right * hw;
                Vector3 c = p - right * hw;
                Vector3 d = p + right * hw;

                int baseIdx = verts.Count;

                float vCoord = distance * 0.05f;
                verts.Add(new RoadVertex(a, Vector3.Up, new Vector2(0f, vCoord), 0f));
                verts.Add(new RoadVertex(b, Vector3.Up, new Vector2(1f, vCoord), 0f));
                verts.Add(new RoadVertex(c, Vector3.Up, new Vector2(0f, vCoord + 0.5f), 0f));
                verts.Add(new RoadVertex(d, Vector3.Up, new Vector2(1f, vCoord + 0.5f), 0f));

                //verts.Add(new RoadVertex(a, Vector3.Up, new Vector2(0f, distance - segLen), 0f));
                //verts.Add(new RoadVertex(b, Vector3.Up, new Vector2(1f, distance - segLen), 0f));
                //verts.Add(new RoadVertex(c, Vector3.Up, new Vector2(0f, distance), 0f));
                //verts.Add(new RoadVertex(d, Vector3.Up, new Vector2(1f, distance), 0f));

                // Two triangles: a-c-b and b-c-d
                idx.Add(baseIdx + 0); idx.Add(baseIdx + 2); idx.Add(baseIdx + 1);
                idx.Add(baseIdx + 1); idx.Add(baseIdx + 2); idx.Add(baseIdx + 3);
            }

            prevPoint = p;
        }
    }

    private static Vector3 ComputeRight(Spline road, float t)
    {
        const float eps = 0.01f;
        float tA = MathHelper.Clamp(t + eps, 0f, 1f);
        float tB = MathHelper.Clamp(t - eps, 0f, 1f);

        Vector3 ahead = road.Evaluate(tA);
        Vector3 behind = road.Evaluate(tB);

        var forward = new Vector3(ahead.X - behind.X, 0f, ahead.Z - behind.Z);
        if (forward.LengthSquared() < 0.0001f) forward = Vector3.Forward;

        forward.Normalize();
        return Vector3.Normalize(Vector3.Cross(Vector3.Up, forward));
    }

    private static void ConnectRings(List<int> idx, int prevRing, int ring, int cols)
    {
        for (int k = 0; k < cols - 1; k++)
        {
            // Triangle 1
            idx.Add(prevRing + k);
            idx.Add(ring + k);
            idx.Add(prevRing + k + 1);

            // Triangle 2
            idx.Add(prevRing + k + 1);
            idx.Add(ring + k);
            idx.Add(ring + k + 1);
        }
    }

}