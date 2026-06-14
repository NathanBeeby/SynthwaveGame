using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Player;

public class Player
{
    public Vector3 Position;
    public float Radius = 1f;
    public Vector3 Velocity;
    public Inventory Inventory = new();
    public PlayerProgress Progress = new();

    public float SpeedMultiplier { get; internal set; }
    public bool IsInvincible { get; internal set; }
    public int MaxHealth { get; internal set; }
}