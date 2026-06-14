using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Models;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Terrain;

public class TerrainChunkBuilder(GraphicsDevice device, TerrainSystem terrain)
{
    private readonly GraphicsDevice _device = device;
    private readonly TerrainSystem _terrain = terrain;
    private const int Divs = 48;

    public void Build(WorldChunk chunk, int chunkSize)
    {
        float ox = chunk.Coord.X * chunkSize;
        float oz = chunk.Coord.Y * chunkSize;
        int stride = Divs + 1;
        float step = (float)chunkSize / Divs;

        // ── Vertices (shared by both solid and wireframe) ─────────────────
        var vertices = new VertexPositionColor[stride * stride];
        int vi = 0;
        for (int z = 0; z <= Divs; z++)
        {
            for (int x = 0; x <= Divs; x++)
            {
                float wx = ox + x * step;
                float wz = oz + z * step;
                float wy = _terrain.GetHeight(wx, wz);
                float t = MathHelper.Clamp(wy / _terrain.HeightScale, 0f, 1f);
                Color c = Color.Lerp(new Color(15, 0, 50), new Color(220, 0, 180), t);
                vertices[vi++] = new VertexPositionColor(new Vector3(wx, wy, wz), c);
            }
        }

        // ── Triangle-list index buffer (for solid terrain pass) ───────────
        var triIndices = new List<int>(Divs * Divs * 6);
        for (int z = 0; z < Divs; z++)
        {
            for (int x = 0; x < Divs; x++)
            {
                int tl = z * stride + x;
                int tr = tl + 1;
                int bl = tl + stride;
                int br = bl + 1;
                triIndices.Add(tl); triIndices.Add(tr); triIndices.Add(bl);
                triIndices.Add(tr); triIndices.Add(br); triIndices.Add(bl);
            }
        }

        // ── Line-list index buffer (for neon wireframe grid pass) ─────────
        var lineIndices = new List<int>(Divs * Divs * 4 + Divs * 4);
        for (int z = 0; z <= Divs; z++)
        {
            for (int x = 0; x <= Divs; x++)
            {
                int cur = z * stride + x;
                if (x < Divs) { lineIndices.Add(cur); lineIndices.Add(cur + 1); }
                if (z < Divs) { lineIndices.Add(cur); lineIndices.Add(cur + stride); }
            }
        }

        // ── Upload solid mesh ─────────────────────────────────────────────
        chunk.TerrainVB = new VertexBuffer(_device,
            typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        chunk.TerrainVB.SetData(vertices);

        chunk.TerrainIB = new IndexBuffer(_device,
            IndexElementSize.ThirtyTwoBits, triIndices.Count, BufferUsage.WriteOnly);
        chunk.TerrainIB.SetData(triIndices.ToArray());

        // ── Upload wireframe grid (stored separately on the chunk) ─────────
        // WorldChunk needs two extra fields: GridIB (IndexBuffer) — see WorldChunk fix below
        chunk.GridIB = new IndexBuffer(_device,
            IndexElementSize.ThirtyTwoBits, lineIndices.Count, BufferUsage.WriteOnly);
        chunk.GridIB.SetData(lineIndices.ToArray());

        chunk.IsBuilt = true;
    }
}