using Synthwave.Core.Classes.Collectables;
using Synthwave.Core.Classes.Core.Physics.Collision;

namespace Synthwave.Core.Classes.AchievementSystem;

public class AchievementContext
{
    public Player.Player Player;
    public CollectableSystem Collectables;
    public CollisionSystem Collision;
}
