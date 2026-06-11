using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Math;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.World;

public class RoadSplineSystem
{
    #region Properties
    public List<Spline> Roads = [];

    // Grid parameters
    private const int GridCount = 8;     // avenues in each axis
    private const float GridSpacing = 600f;  // distance between avenues (world units)
    private const float GridOffset = -(GridCount / 2f) * GridSpacing; // centre on origin

    // Stores intersection world positions for connectors / roundabouts
    private Vector3[,] _grid;
    #endregion

    #region Methods
    public void Generate(int seed = 1337)
    {
        Roads.Clear();
        _grid = new Vector3[GridCount + 1, GridCount + 1];

        var rng = new Random(seed);

        // ── 1. Build intersection grid ────────────────────────────────────
        for (int row = 0; row <= GridCount; row++)
        {
            for (int col = 0; col <= GridCount; col++)
            {
                float x = GridOffset + col * GridSpacing
                          + rng.NextSingle(-40f, 40f);  // slight organic variation
                float z = GridOffset + row * GridSpacing
                          + rng.NextSingle(-40f, 40f);
                _grid[row, col] = new Vector3(x, 0f, z);
            }
        }

        // ── 2. Horizontal avenues (row by row) ───────────────────────────
        for (int row = 0; row <= GridCount; row++)
        {
            var spline = new Spline();
            for (int col = 0; col <= GridCount; col++)
                spline.Points.Add(_grid[row, col]);
            Roads.Add(spline);
        }

        // ── 3. Vertical avenues (column by column) ───────────────────────
        for (int col = 0; col <= GridCount; col++)
        {
            var spline = new Spline();
            for (int row = 0; row <= GridCount; row++)
                spline.Points.Add(_grid[row, col]);
            Roads.Add(spline);
        }

        // ── 4. Diagonal connectors (every other intersection) ────────────
        for (int row = 0; row < GridCount; row++)
        {
            for (int col = 0; col < GridCount; col++)
            {
                if ((row + col) % 3 != 0) continue;   // sparse diagonals

                // NW → SE
                var diag = new Spline();
                diag.Points.Add(_grid[row, col]);
                // Mid-control point: midpoint + small perpendicular bulge
                Vector3 mid = (_grid[row, col] + _grid[row + 1, col + 1]) * 0.5f
                              + new Vector3(rng.NextSingle(-60f, 60f), 0f,
                                            rng.NextSingle(-60f, 60f));
                diag.Points.Add(mid);
                diag.Points.Add(_grid[row + 1, col + 1]);
                Roads.Add(diag);
            }
        }

        // ── 5. Roundabout loops at random intersections ───────────────────
        for (int i = 0; i < 8; i++)
        {
            int row = rng.Next(1, GridCount);
            int col = rng.Next(1, GridCount);
            AddRoundabout(_grid[row, col], rng.NextSingle(80f, 160f));
        }
    }



    private void AddRoundabout(Vector3 centre, float radius)
    {
        var circle = new Spline();
        const int Segs = 16;
        for (int i = 0; i <= Segs; i++)   // +1 so the spline closes
        {
            float a = MathHelper.TwoPi * i / Segs;
            circle.Points.Add(centre + new Vector3(
                MathF.Cos(a) * radius,
                0f,
                MathF.Sin(a) * radius));
        }
        Roads.Add(circle);
    }
    


    private static Spline GenerateMainRoad(Random r)
    {
        Spline s = new();

        Vector3 start = new(r.Next(-2000, 2000), 0, r.Next(-2000, 2000));
        Vector3 end = new(r.Next(-2000, 2000), 0, r.Next(-2000, 2000));

        for (int i = 0; i < 6; i++)
        {
            s.Points.Add(Vector3.Lerp(start, end, i / 5f) +
                         new Vector3(r.Next(-200, 200), 0, r.Next(-200, 200)));
        }

        return s;
    }

    private void GenerateRoundabouts(Random r)
    {
        // simplified: circular splines
        for (int i = 0; i < 6; i++)
        {
            Spline circle = new();

            Vector3 center = new(r.Next(-1000, 1000), 0, r.Next(-1000, 1000));
            float radius = r.Next(80, 200);

            for (int j = 0; j < 12; j++)
            {
                float a = j / 12f * MathF.Tau;
                circle.Points.Add(center + new Vector3(MathF.Cos(a), 0, MathF.Sin(a)) * radius);
            }

            Roads.Add(circle);
        }
    }
    #endregion
}
