namespace Synthwave.Core.Classes.PowerUps;

public abstract class PowerUp(string id)
{
    public string Id { get; } = id;
    public abstract void Activate(Player.Player player);
    public abstract void Deactivate(Player.Player player);
}
