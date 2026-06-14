using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Core.Physics.Collision;
using System;

namespace Synthwave.Core.Classes.Collectables;

public class CollectableItem : Collidable
{
    public CollectableType ItemType { get; }
    public int Quantity { get; }

    public bool IsCollected { get; private set; }

    public CollectableItem(CollectableType itemType, int quantity, Vector3 position, float radius = 1f)
    {
        ItemType = itemType;
        Quantity = quantity;

        Position = position;
        Collider = new SphereCollider(position, radius);
    }

    public override void OnCollision(Player.Player player)
    {
        if (IsCollected) return;

        IsCollected = true;

        player.Inventory.Add(ItemType, Quantity);

        // trigger effects here (sound, particles, etc.)
        Console.WriteLine($"Collected {Quantity} {ItemType}");
    }
}