using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.World;
using System;

namespace Synthwave.Core.Classes.Graphics.HUD;

public class VehicleHUD
{
    #region Properties
    private SpriteFont _font;
    #endregion

    #region Properties
    public void Load(ContentManager content)
    {
        _font = content.Load<SpriteFont>("Fonts/Hud");
    }

    public void Draw(SpriteBatch spriteBatch, VehicleController vehicle)
    {
        spriteBatch.Begin();

        spriteBatch.DrawString(_font, $"Speed: {(int)MathF.Abs(vehicle.CurrentSpeed)} km/h", new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(_font, $"Gear: {vehicle.CurrentGear}", new Vector2(20, 50), Color.White);
        spriteBatch.DrawString(_font, $"RPM: {(int)vehicle.EngineRPM}", new Vector2(20, 80), Color.White);
        spriteBatch.DrawString(_font, $"Transmission: {vehicle.Transmission}", new Vector2(20, 110), Color.White);
        spriteBatch.DrawString(_font, $"View: {vehicle.ViewMode}", new Vector2(20, 140), Color.White);
        spriteBatch.DrawString(_font, $"NOS: {(int)vehicle.NitrousAmount}%", new Vector2(20, 170), vehicle.NitrousEnabled ? Color.HotPink : Color.Gray);

        spriteBatch.End();
    }
    #endregion
}
