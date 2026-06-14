using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Player;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public abstract class Collidable
{
    public Vector3 Position { get; protected set; }
    public SphereCollider Collider { get; protected set; }

    public abstract void OnCollision(Player.Player player);
}
