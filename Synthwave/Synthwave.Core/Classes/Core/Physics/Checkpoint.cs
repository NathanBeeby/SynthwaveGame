using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Core.Physics.Collision;
using System;

namespace Synthwave.Core.Classes.Core.Physics;

public class Checkpoint : Collidable
{
    public int Index { get; }
    public bool IsActivated { get; private set; }

    public Checkpoint(Vector3 position, float radius, int index)
    {
        Position = position;
        Collider = new SphereCollider(position, radius);
        Index = index;
    }

    public override void OnCollision(Player.Player player)
    {
        if (IsActivated) return;

        IsActivated = true;

        //playerProgress.LastCheckpointIndex = Index;
        //playerProgress.CheckpointPosition = Position;

        Console.WriteLine($"Checkpoint {Index} reached!");
    }
}
