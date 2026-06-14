using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Physics.Collision;

namespace Synthwave.Core.Classes.Core.Physics;

public class SolidObject : Collidable
{
    public SolidObject(Vector3 position, float radius)
    {
        Position = position;
        Collider = new SphereCollider(position, radius);
    }

    public override void OnCollision(Player.Player player)
    {
        // Simple response: push player back or stop movement
        player.Velocity = Vector3.Zero;
    }
}