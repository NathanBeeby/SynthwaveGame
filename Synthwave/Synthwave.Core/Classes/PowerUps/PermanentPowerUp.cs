namespace Synthwave.Core.Classes.PowerUps;

public class PermanentPowerUp(string id) : PowerUp(id)
{
    public override void Activate(Player.Player player) => player.MaxHealth += 20;
    
    public override void Deactivate(Player.Player player)
    {
        // permanent = no removal
    }
}
