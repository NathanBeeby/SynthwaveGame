using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Vehicle;
using Synthwave.Core.Classes.World.Weather;

namespace Synthwave.Core.Classes.Graphics.HUD;

public class HUD
{
    #region Properties
    private VehicleHUD vHUD;
    #endregion

    #region Methods
    public void Load(ContentManager content)
    {
        vHUD ??= new VehicleHUD();
        vHUD.Load(content);
    }

    public void Draw(SpriteBatch spriteBatch, VehicleController vehicle, WeatherSystem weather)
    {
        vHUD.Draw(spriteBatch, vehicle, weather);
    }
    #endregion
}
