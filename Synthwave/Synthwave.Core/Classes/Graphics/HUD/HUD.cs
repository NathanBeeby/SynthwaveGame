using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Vehicle;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;

namespace Synthwave.Core.Classes.Graphics.HUD;

public class HUD
{
    #region Properties
    private VehicleHUD vHUD;
    private VehicleController _vehicleController;
    private Camera3D _camera;
    private WeatherSystem _weather;
    #endregion

    #region Methods
    public void Load(ContentManager content, VehicleController controller, Camera3D camera, WeatherSystem weather)
    {
        vHUD ??= new VehicleHUD();
        _vehicleController = controller;
        _camera = camera;
        _weather = weather;

        vHUD.Load(content, _vehicleController, _camera, _weather);
    }

    public void Update(GameTime gameTime)
    {
        vHUD.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        vHUD.Draw(spriteBatch);
    }
    #endregion
}
