using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthwave.Core.Classes.PowerUps;

public class PowerUpManager
{
    private readonly Dictionary<string, PowerUp> _registry = [];
    private readonly Dictionary<string, float> _timers = [];

    public void Register(PowerUp powerUp) => _registry[powerUp.Id] = powerUp;
    
    public void Activate(string id, Player.Player player)
    {
        if (!_registry.TryGetValue(id, out var powerUp))
            return;

        powerUp.Activate(player);

        if (powerUp is TemporaryPowerUp)
        {
            _timers[id] = 0f;
        }
    }

    public void Update(float dt, Player.Player player)
    {
        var keys = _timers.Keys.ToList();

        foreach (var id in keys)
        {
            _timers[id] += dt;

            // example duration check
            if (_timers[id] > 5f)
            {
                _registry[id].Deactivate(player);
                _timers.Remove(id);
            }
        }
    }
}

/*
 
 How to connect to shop:
var selectedId = shop.GetSelected(ShopItemType.VehicleUpgrade);

powerUpManager.Activate(selectedId, player);

// Buy Item:
shop.Buy("nitro_boost");

// Select Item:
shop.Select("nitro_boost");

// Activate In Game:
powerUpManager.Activate(shop.GetSelected(ShopItemType.VehicleUpgrade), player);

 */