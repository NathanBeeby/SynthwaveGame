using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Enums;
using System;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Store;

public class ShopManager
{
    #region Properties
    private readonly Dictionary<string, ShopItem> _items = [];

    public SavedData Data { get; private set; }
    private readonly Storage _storage = new();

    public event Action<string> OnPurchased;
    public event Action<string> OnSelected;
    #endregion

    #region Constructor
    public ShopManager() =>  Data = _storage.Load();
    #endregion

    #region Methods
    public void RegisterItem(ShopItem item) => _items[item.Id] = item;

    public bool Buy(string itemId)
    {
        if (!_items.TryGetValue(itemId, out var item))
            return false;

        if (Data.Currency < item.Cost)
            return false;

        Data.Currency -= item.Cost;

        if (!Data.OwnedItems.ContainsKey(itemId))
            Data.OwnedItems[itemId] = 0;

        Data.OwnedItems[itemId]++;

        Save();

        OnPurchased?.Invoke(itemId);
        return true;
    }


    public void Select(string itemId)
    {
        if (!_items.TryGetValue(itemId, out var item)) return;
        if (!Data.OwnedItems.ContainsKey(itemId)) return;

        Data.SelectedItems[item.Type] = itemId;
        Save();
        OnSelected?.Invoke(itemId);
    }

    public string GetSelected(ShopItemType type) => Data.SelectedItems.TryGetValue(type, out var id)? id: null;

    public bool Owns(string itemId) => Data.OwnedItems.ContainsKey(itemId);

    private void Save() => _storage.Save(Data);
    #endregion
}
