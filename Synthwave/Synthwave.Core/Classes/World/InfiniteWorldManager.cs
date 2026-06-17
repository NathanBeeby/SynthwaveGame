using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Math;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World.Roads;
using Synthwave.Core.Classes.World.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthwave.Core.Classes.World;

public class InfiniteWorldManager(GraphicsDevice device, TerrainSystem terrain)
{
    private readonly Dictionary<Point, WorldChunk> _chunks = [];
    private readonly TerrainChunkBuilder _terrainBuilder = new(device, terrain);
    private readonly NeonRoadMeshBuilder _roadBuilder = new(device, terrain); // switched from RoadMeshBuilder
    public static int ChunkSize = 1000;
    public int ViewDistance = 3;
    public int EvictDistance = 5;

    public void Update(Vector3 cameraPos, RoadSplineSystem roads)
    {
        int cx = ChunkIndex(cameraPos.X);
        int cz = ChunkIndex(cameraPos.Z);

        for (int x = cx - ViewDistance; x <= cx + ViewDistance; x++)
            for (int z = cz - ViewDistance; z <= cz + ViewDistance; z++)
            {
                var chunk = GetOrCreate(x, z);
                if (chunk.IsBuilt) continue;
                _terrainBuilder.Build(chunk, ChunkSize);
                var localRoads = roads.Roads.Where(r => RoadTouchesChunk(r, x, z)).ToList();


                float minX = x * ChunkSize;
                float minZ = z * ChunkSize;

                BoundingBox bounds =new(new Vector3(minX,-10000,minZ),new Vector3(minX + ChunkSize,10000,minZ + ChunkSize));

                _roadBuilder.Build(chunk,roads);
                chunk.IsBuilt = true;
            }

        var toEvict = _chunks.Values.Where(c => ChebyshevDist(c.Coord, cx, cz) > EvictDistance).ToList();
        foreach (var c in toEvict)
        {
            c.Dispose();
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

    private int ChunkIndex(float worldCoord) => (int)MathF.Floor(worldCoord / ChunkSize);

    private static int ChebyshevDist(Point p, int cx, int cz) => Math.Max(Math.Abs(p.X - cx), Math.Abs(p.Y - cz));

    private bool RoadTouchesChunk(Spline road, int cx, int cz)
    {
        float minX = cx * ChunkSize, maxX = minX + ChunkSize;
        float minZ = cz * ChunkSize, maxZ = minZ + ChunkSize;
        for (int i = 0; i <= 20; i++)
        {
            Vector3 p = road.Evaluate(i / 20f);
            if (p.X >= minX && p.X <= maxX && p.Z >= minZ && p.Z <= maxZ) return true;
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
}