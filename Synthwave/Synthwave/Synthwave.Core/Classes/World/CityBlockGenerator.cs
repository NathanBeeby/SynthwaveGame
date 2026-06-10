using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World;

public class CityBlockGenerator
{
    #region Classes
    public class Block
    {
        public Vector3 Position;
        public Vector2 Size;
        public int Density;
    }
    #endregion

    #region Properties
    public List<Block> Blocks = new();
    #endregion

    #region Methods
    public void Generate(RoadSplineSystem roads, TerrainSystem terrain)
    {
        Blocks.Clear();
        var rng = new Random(1337);

        foreach (var road in roads.Roads)
        {
            // Sample blocks along each road at intervals
            for (float t = 0f; t < 1f; t += 0.08f)
            {
                if (rng.NextDouble() > 0.55) continue;

                Vector3 p = road.Evaluate(t);
                p.Y = terrain.GetHeight(p.X, p.Z);

                Blocks.Add(new Block
                {
                    Position = p,
                    Size = new Vector2(rng.Next(20, 80), rng.Next(20, 80)),
                    Density = rng.Next(1, 12)
                });
            }
        }
    }
    #endregion
}
