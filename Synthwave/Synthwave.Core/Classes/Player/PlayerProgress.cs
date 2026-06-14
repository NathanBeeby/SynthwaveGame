using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Player;

public class PlayerProgress
{
    public int LastCheckpointIndex { get; set; } = -1;
    public Vector3 CheckpointPosition { get; set; }
}
