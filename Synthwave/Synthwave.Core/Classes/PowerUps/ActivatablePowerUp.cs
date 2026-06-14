namespace Synthwave.Core.Classes.PowerUps;

public class ActivatablePowerUp(string id) : PowerUp(id)
{
    private bool _active;

    public override void Activate(Player.Player player)
    {
        _active = !_active;
        player.IsInvincible = _active;
    }

    public override void Deactivate(Player.Player player)
    {
        player.IsInvincible = false;
        _active = false;
    }
}
