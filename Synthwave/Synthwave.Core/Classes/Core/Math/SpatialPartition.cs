using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core.Math;

public class SpatialPartition<T>(float cellSize, Func<T, Vector2> getPos)
{
    #region Properties
    private readonly Dictionary<(int, int), List<T>> grid = [];
    private readonly float cellSize = cellSize;
    private readonly Func<T, Vector2> getPos = getPos;
    #endregion

    #region Methods
    public void Insert(T item)
    {
        var p = getPos(item);
        var key = Hash(p);

        if (!grid.ContainsKey(key)) grid[key] = [];

        grid[key].Add(item);
    }

    public List<T> Query(Vector2 pos, float radius)
    {
        List<T> result = [];

        int minX = (int)((pos.X - radius) / cellSize);
        int maxX = (int)((pos.X + radius) / cellSize);
        int minY = (int)((pos.Y - radius) / cellSize);
        int maxY = (int)((pos.Y + radius) / cellSize);

        for (int x = minX; x <= maxX; x++)
            for (int y = minY; y <= maxY; y++)
            {
                if (grid.TryGetValue((x, y), out var list))
                    result.AddRange(list);
            }

        return result;
    }

    private (int, int) Hash(Vector2 p)
    {
        return ((int)(p.X / cellSize), (int)(p.Y / cellSize));
    }
    #endregion
}
