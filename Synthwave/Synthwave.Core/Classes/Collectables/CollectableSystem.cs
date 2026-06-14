using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.World;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Collectables;

public class CollectableSystem(TerrainSystem terrain, Func<Vector3, bool> isOnRoad = null)
{
    #region Properties
    private readonly List<CollectableItem> _items = [];
    private readonly TerrainSystem _terrain = terrain;
    private readonly Func<Vector3, bool> _isOnRoad = isOnRoad ?? ((pos) => false); 
    private readonly Random _random = new();
    #endregion

    #region Spawn
    public void SpawnRandomCollectables(int count, CollectableType type)
    {
        for (int i = 0; i < count; i++)
        {
            float x = (float)_random.NextDouble() * _terrain.Width;
            float z = (float)_random.NextDouble() * _terrain.Length;

            float y = _terrain.GetHeight(x, z);
            _items.Add(new CollectableItem(type, 1, new Vector3(x, y + 1f, z)));
        }
    }

    #endregion

    #region Update / Retrieval
    public List<CollectableItem> CheckCollection(Vector3 playerPosition, float collectRadius = 2f)
    {
        List<CollectableItem> collected = [];

        for (int i = _items.Count - 1; i >= 0; i--)
        {
            if (Vector3.Distance(playerPosition, _items[i].Position) <= collectRadius)
            {
                collected.Add(_items[i]);
                _items.RemoveAt(i);
            }
        }

        return collected;
    }

    public IEnumerable<CollectableItem> Items => _items;
    #endregion
}
