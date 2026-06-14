using Synthwave.Core.Classes.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Synthwave.Core.Classes.Player;

public class Inventory
{
    #region Properties
    private readonly Dictionary<CollectableType, int> _items = [];
    #endregion

    #region Methods
    public void Add(CollectableType type, int amount)
    {
        if (_items.ContainsKey(type)) _items[type] += amount;
        else _items[type] = amount;
    }

    public bool Has(CollectableType type, int amount = 1) => _items.TryGetValue(type, out int value) && value >= amount;

    public bool Consume(CollectableType type, int amount)
    {
        if (!Has(type, amount)) return false;

        _items[type] -= amount;
        if (_items[type] <= 0) _items.Remove(type);

        return true;
    }

    public int GetAmount(CollectableType type) => _items.TryGetValue(type, out var value) ? value : 0;

    internal int GetTotalItems() => Items.Values.Sum();
    
    public IReadOnlyDictionary<CollectableType, int> Items => _items;
    #endregion
}
