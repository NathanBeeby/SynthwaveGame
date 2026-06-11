using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World.Terrain;

public class TerrainChunkBuilder(GraphicsDevice device, TerrainSystem terrain)
{
    #region Properties
    private readonly GraphicsDevice _device = device;
    private readonly TerrainSystem _terrain = terrain;

    // Resolution: 48 quads per side gives a dense enough grid to look good
    private const int Divs = 48;
    #endregion

    #region Methods
    public void Build(WorldChunk chunk, int chunkSize)
    {
        float ox = chunk.Coord.X * chunkSize;   // world-space origin
        float oz = chunk.Coord.Y * chunkSize;   // (Point.Y stores Z chunk index)

        int stride = Divs + 1;
        float step = (float)chunkSize / Divs;

        // ── Vertices ─────────────────────────────────────────────────────
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

                // Deep purple valley → hot magenta peak
                Color c = Color.Lerp(
                    new Color(15, 0, 50),
                    new Color(220, 0, 180),
                    t);

                vertices[vi++] = new VertexPositionColor(new Vector3(wx, wy, wz), c);
            }
        }

        // ── Wireframe index buffer (line list) ───────────────────────────
        // Each quad contributes 2 edges (right + down) to avoid duplicates.
        // Corner quads add the missing far edges.
        var lineIndices = new List<int>(Divs * Divs * 4 + Divs * 4);

        for (int z = 0; z <= Divs; z++)
        {
            for (int x = 0; x <= Divs; x++)
            {
                int cur = z * stride + x;

                // Horizontal edge (→)
                if (x < Divs)
                {
                    lineIndices.Add(cur);
                    lineIndices.Add(cur + 1);
                }
                // Vertical edge (↓)
                if (z < Divs)
                {
                    lineIndices.Add(cur);
                    lineIndices.Add(cur + stride);
                }
            }
        }

        int[] idxArray = lineIndices.ToArray();

        chunk.TerrainVB = new VertexBuffer(_device,
            typeof(VertexPositionColor), vertices.Length, BufferUsage.WriteOnly);
        chunk.TerrainVB.SetData(vertices);

        chunk.TerrainIB = new IndexBuffer(_device,
            IndexElementSize.ThirtyTwoBits, idxArray.Length, BufferUsage.WriteOnly);
        chunk.TerrainIB.SetData(idxArray);

        chunk.IsBuilt = true;
    }
    #endregion
}

