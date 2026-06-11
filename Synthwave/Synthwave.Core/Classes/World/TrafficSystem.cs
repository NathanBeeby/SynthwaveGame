using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Models;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World;

public class TrafficSystem
{
    #region Properties
    public List<Car> Cars = [];

    private TerrainSystem _terrain;
    #endregion

    #region Methods
    public void Spawn(RoadSplineSystem roads, TerrainSystem terrain = null)
    {
        _terrain = terrain;
        Cars.Clear();
        var rng = new Random(1337);

        foreach (var road in roads.Roads)
        {
            // Fewer cars per road so the scene isn't flooded
            int count = rng.Next(2, 6);
            for (int i = 0; i < count; i++)
            {
                Cars.Add(new Car
                {
                    Road = road,
                    T = (float)rng.NextDouble(),
                    Speed = 0.03f + (float)rng.NextDouble() * 0.08f
                });
            }
        }
    }

    public void Update(float dt)
    {
        foreach (var c in Cars)
        {
            c.T = (c.T + dt * c.Speed) % 1f;

            Vector3 p = c.Road.Evaluate(c.T);

            // Snap to terrain if available
            float groundY = _terrain != null
                ? _terrain.GetHeight(p.X, p.Z)
                : p.Y;

            c.Position = new Vector3(p.X, groundY + 1.2f, p.Z);
        }
    }
    #endregion
}
