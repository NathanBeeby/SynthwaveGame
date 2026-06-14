using Synthwave.Core.Classes.Core.Enums;

namespace Synthwave.Core.Classes.Store;

public class ShopItem
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string IconKey { get; set; }
    public int Cost { get; set; }
    public ShopItemType Type { get; set; }
}
