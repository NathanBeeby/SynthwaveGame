namespace Synthwave.Core.Classes.PowerUps;

public class TemporaryPowerUp(string id, float duration) : PowerUp(id)
{
    private float _duration = duration;

    public override void Activate(Player.Player player) => player.SpeedMultiplier *= 2f;
    public override void Deactivate(Player.Player player) => player.SpeedMultiplier /= 2f;
}
