using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Physics.Collision;

namespace Synthwave.Core.Classes.Player;

public class Enemy : Collidable
{
    public int DamageAmount { get; private set; }
    public int Health { get; private set; }

    public bool IsDead => Health <= 0;

    public Enemy(Vector3 position, float radius, int health, int damage)
    {
        Position = position;
        Collider = new SphereCollider(position, radius);

        Health = health;
        DamageAmount = damage;
    }

    public override void OnCollision(Player player)
    {
        if (IsDead) return;

        //player.Damage(DamageAmount);
    }

    public void TakeDamage(int amount)
    {
        Health -= amount;
        if (Health < 0) Health = 0;
    }
}
