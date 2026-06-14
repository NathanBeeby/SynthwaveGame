using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World.Weather;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World;

public class CityBlockGenerator
{
    #region Properties
    public List<Block> Blocks = [];
    #endregion

    #region Methods
    public void Generate(RoadSplineSystem roads, TerrainSystem terrain)
    {
        Blocks.Clear();
        var rng = new Random(1337);

        foreach (var road in roads.Roads)
        {
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

    public void Update(WeatherSystem weather)
    {
        // Tint is available for future use; no per-frame allocation needed
        Color tint = new(weather.AmbientTint.ToVector3() * weather.Visibility);
        _ = tint; // suppress unused warning until tint is wired into Draw
    }
    #endregion
}