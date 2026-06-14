using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Physics.Collision;

namespace Synthwave.Core.Classes.Player;

public class Trap : Collidable
{
    public int Damage { get; private set; }
    public bool IsActive { get; private set; } = true;

    public Trap(Vector3 position, float radius, int damage)
    {
        Position = position;
        Collider = new SphereCollider(position, radius);
        Damage = damage;
    }

    public override void OnCollision(Player player)
    {
        if (!IsActive) return;

       // player.Damage(Damage);

        // Optional: single-use trap
        // IsActive = false;
    }
}