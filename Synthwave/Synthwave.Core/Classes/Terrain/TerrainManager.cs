using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Models;

public class TerrainManager(HeightMap map, GraphicsDevice graphics)
{
    #region Properties
    private readonly Dictionary<Point, TerrainChunk> chunks = new();
    private readonly HeightMap map = map;
    private readonly GraphicsDevice graphics = graphics;
    #endregion

    #region Methods
    public void Update(Vector3 playerPos)
    {
        int cx = (int)MathF.Floor(playerPos.X / TerrainChunk.Size);
        int cz = (int)MathF.Floor(playerPos.Z / TerrainChunk.Size);

        const int viewDistance = 3;

        for (int z = -viewDistance; z <= viewDistance; z++)
        {
            for (int x = -viewDistance; x <= viewDistance; x++)
            {
                Point p = new(cx + x, cz + z);
                if (!chunks.ContainsKey(p)) chunks[p] = new TerrainChunk(p, map);
            }
        }
    }

    public void Draw(BasicEffect effect)
    {
        foreach (var chunk in chunks.Values)
        {
            effect.World = Matrix.Identity;

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                graphics.DrawUserPrimitives(PrimitiveType.TriangleList,chunk.Vertices,0,chunk.Vertices.Length / 3);
            }
        }
    }
    #endregion
}