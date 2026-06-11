using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;

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

    public void Draw(SpriteBatch spriteBatch, VehicleController vehicle)
    {
        vHUD.Draw(spriteBatch, vehicle);
    }
    #endregion
}
