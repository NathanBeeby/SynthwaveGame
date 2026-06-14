using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Models;
using Synthwave.Core.Classes.World.Weather;
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

    public void Update(float dt, WeatherSystem weather, TerrainSystem terrain)
    {
        foreach (var c in Cars)
        {
            c.T = (c.T + dt * c.Speed) % 1f;
            Vector3 p = c.Road.Evaluate(c.T);

            float groundY = terrain != null ? terrain.GetHeight(p.X, p.Z) : p.Y;
            float puddle = terrain != null ? terrain.GetWaterLevel(p.X, p.Z) : 0f;

            float hydro = puddle * weather.HydroplaningFactor;

            float friction = weather.FrictionMultiplier * (1f - hydro);

            // Reduce car speed if hydroplaning
            float speedFactor = MathHelper.Clamp(friction, 0.5f, 1f);
            c.Speed *= speedFactor;

            c.Position = new Vector3(p.X, groundY + 1.2f, p.Z);
        }
    }
    #endregion
}
