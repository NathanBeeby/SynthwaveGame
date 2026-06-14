using Synthwave.Core.Classes.Player;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public class CollisionSystem
{
    private readonly List<Collidable> _objects = [];

    public void Register(Collidable obj)
    {
        _objects.Add(obj);
    }

    public void Unregister(Collidable obj)
    {
        _objects.Remove(obj);
    }

    public void Check(Player.Player player)
    {
        var playerCollider = new SphereCollider(player.Position, player.Radius);

        foreach (var obj in _objects)
        {
            if (obj is Enemy enemy && enemy.IsDead)
                continue;

            if (playerCollider.Intersects(obj.Collider))
            {
                obj.OnCollision(player);
            }
        }
    }
}