using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Math;
using Synthwave.Core.Classes.Roads;
using Synthwave.Core.Classes.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthwave.Core.Classes.World;

public class InfiniteWorldManager(GraphicsDevice device, TerrainSystem terrain)
{
    #region Properties
    private readonly Dictionary<Point, WorldChunk> _chunks = new();
    private readonly TerrainChunkBuilder _terrainBuilder = new TerrainChunkBuilder(device, terrain);
    private readonly RoadMeshBuilder _roadBuilder = new RoadMeshBuilder(device, terrain);

    public int ChunkSize = 1000;
    public int ViewDistance = 3;     // chunks to load in each direction
    public int EvictDistance = 5;    // chunks beyond this are unloaded
    #endregion

    #region Methods
    public void Update(Vector3 cameraPos, RoadSplineSystem roads)
    {
        int cx = ChunkIndex(cameraPos.X);
        int cz = ChunkIndex(cameraPos.Z);

        // ── Build missing chunks in view range ────────────────────────────
        for (int x = cx - ViewDistance; x <= cx + ViewDistance; x++)
        {
            for (int z = cz - ViewDistance; z <= cz + ViewDistance; z++)
            {
                var chunk = GetOrCreate(x, z);
                if (chunk.IsBuilt) continue;

                _terrainBuilder.Build(chunk, ChunkSize);

                // All roads whose spline spans over this chunk's world rect
                var localRoads = roads.Roads
                    .Where(r => RoadTouchesChunk(r, x, z))
                    .ToList();

                _roadBuilder.Build(chunk, localRoads);
                // IsBuilt already set to true by TerrainChunkBuilder.Build
            }
        }

        // ── Evict distant chunks ──────────────────────────────────────────
        var toEvict = _chunks.Values
            .Where(c => ChebyshevDist(c.Coord, cx, cz) > EvictDistance)
            .ToList();

        foreach (var c in toEvict)
        {
            c.Dispose();    // frees GPU buffers, sets IsBuilt=false
            _chunks.Remove(c.Coord);
        }
    }

    public List<WorldChunk> GetVisibleChunks(Vector3 cameraPos)
    {
        int cx = ChunkIndex(cameraPos.X);
        int cz = ChunkIndex(cameraPos.Z);

        var result = new List<WorldChunk>();
        for (int x = cx - ViewDistance; x <= cx + ViewDistance; x++)
            for (int z = cz - ViewDistance; z <= cz + ViewDistance; z++)
                result.Add(GetOrCreate(x, z));
        return result;
    }


    // ── Helpers ──────────────────────────────────────────────────────────


    private int ChunkIndex(float worldCoord)
        => (int)MathF.Floor(worldCoord / ChunkSize);

    private static int ChebyshevDist(Point p, int cx, int cz)
        => System.Math.Max(System.Math.Abs(p.X - cx), System.Math.Abs(p.Y - cz));

    private bool RoadTouchesChunk(Spline road, int cx, int cz)
    {
        float minX = cx * ChunkSize;
        float maxX = minX + ChunkSize;
        float minZ = cz * ChunkSize;
        float maxZ = minZ + ChunkSize;

        for (int i = 0; i <= 20; i++)
        {
            Vector3 p = road.Evaluate(i / 20f);
            if (p.X >= minX && p.X <= maxX && p.Z >= minZ && p.Z <= maxZ)
                return true;
        }
        return false;
    }

    private WorldChunk GetOrCreate(int x, int z)
    {
        var key = new Point(x, z);
        if (!_chunks.TryGetValue(key, out var chunk))
        {
            chunk = new WorldChunk { Coord = key, IsBuilt = false };
            _chunks[key] = chunk;
        }
        return chunk;
    }
    #endregion
}