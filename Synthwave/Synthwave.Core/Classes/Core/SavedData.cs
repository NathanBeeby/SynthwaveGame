using Synthwave.Core.Classes.AchievementSystem;
using Synthwave.Core.Classes.Core.Enums;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core;

public class SavedData
{
    public List<Achievement> Achievements { get; set; } = [];
    // TODO: Add more of the saved data, e.g. Vehicle, Permanent Upgrades, Power Ups, Asset Packs, Current Points, Current Health, etc.
    // itemId → quantity
    public Dictionary<string, int> OwnedItems { get; set; } = new();

    // currently selected item per category
    public Dictionary<ShopItemType, string> SelectedItems { get; set; } = new();

    public int Currency { get; set; }
}
