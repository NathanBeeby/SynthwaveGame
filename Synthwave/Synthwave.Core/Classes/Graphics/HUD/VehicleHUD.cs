using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Vehicle;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;
using System;

namespace Synthwave.Core.Classes.Graphics.HUD;

public class VehicleHUD
{
    #region Fields

    private float _wheelRotation;
    private float _speedRotation;
    private float _rpmRotation;

    private const float MaxWheelRotation = MathHelper.PiOver2;

    private readonly float NeedleStartAngle = MathHelper.ToRadians(-130);
    private readonly float NeedleEndAngle = MathHelper.ToRadians(130);

    private const float MaxRPM = 7000f;
    private const float MaxSpeed = 220f;

    private VehicleController vehiclecontroller;
    private Camera3D _camera;
    private WeatherSystem _weather;

    private SpriteFont _font;

    private Texture2D _interior;
    private Texture2D _wheel;
    private Texture2D _speedNeedle;
    private Texture2D _rpmNeedle;

    #endregion

    #region Load
    public void Load(ContentManager content, VehicleController controller, Camera3D camera, WeatherSystem weather)
    {
        vehiclecontroller = controller;
        _camera = camera;
        _weather = weather;

        _font = content.Load<SpriteFont>("Fonts/Hud");

        _interior = content.Load<Texture2D>("2DSprites/Interior");
        _wheel = content.Load<Texture2D>("2DSprites/SteeringWheel");

        _speedNeedle = content.Load<Texture2D>("2DSprites/SpeedNeedle");
        _rpmNeedle = content.Load<Texture2D>("2DSprites/SpeedNeedle"); // IMPORTANT: separate texture
    }
    #endregion

    #region Methods
    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        UpdateSteering(dt);
        UpdateGauges(dt);
    }


    private void UpdateSteering(float dt)
    {
        float steerInput = (vehiclecontroller.State.SteeringInput);
        float targetWheel = steerInput * MaxWheelRotation;

        _wheelRotation = MathHelper.Lerp(_wheelRotation, targetWheel, dt * 10f);
    }

    private void UpdateGauges(float dt)
    {
        float speedPercent =  MathHelper.Clamp(vehiclecontroller.State.CurrentSpeed / MaxSpeed, 0f, 1f);
        float rpmPercent =  MathHelper.Clamp(vehiclecontroller.State.EngineRPM / MaxRPM, 0f, 1f);

        float targetSpeedAngle = MapGauge(speedPercent);
        float targetRPMAngle = MapGauge(rpmPercent);

        _speedRotation = MathHelper.Lerp(_speedRotation, targetSpeedAngle, dt * 8f);
        _rpmRotation = MathHelper.Lerp(_rpmRotation, targetRPMAngle, dt * 8f);
    }

    private float MapGauge(float percent) => MathHelper.Lerp(NeedleStartAngle, NeedleEndAngle, percent);

    #region Draw
    public void Draw(SpriteBatch spriteBatch)
    {
        if (vehiclecontroller.ViewMode == CameraMode.FirstPerson) DrawFirstPerson(spriteBatch);
        else DrawThirdPerson(spriteBatch);
    }

    private void DrawFirstPerson(SpriteBatch spriteBatch)
    {
        GraphicsDevice device = spriteBatch.GraphicsDevice;

        int width = device.Viewport.Width;
        int height = device.Viewport.Height;

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        spriteBatch.Draw(_interior, new Rectangle(0, height - _interior.Height, width, _interior.Height + 50), Color.White);

        Vector2 wheelPos = new(width * 0.5f, height - 50);
        Vector2 wheelOrigin = new(_wheel.Width * 0.5f, _wheel.Height * 0.5f);
        spriteBatch.Draw(_wheel,wheelPos,null,Color.White,_wheelRotation,wheelOrigin,2f,SpriteEffects.None,0f);

        Vector2 rpmPos = new(width * 0.585f, height - 140);

        Vector2 rpmOrigin = new(_rpmNeedle.Width * 0.5f,_rpmNeedle.Height * 0.9f); // FIXED pivot
        spriteBatch.Draw(_rpmNeedle,rpmPos,null,Color.White,_rpmRotation,rpmOrigin,1.5f,SpriteEffects.None,0f);

        Vector2 speedPos = new(width * 0.43f, height - 140);

        Vector2 speedOrigin = new(_speedNeedle.Width * 0.5f,_speedNeedle.Height * 0.9f); // FIXED pivot

        spriteBatch.Draw(_speedNeedle,speedPos,null,Color.White,_speedRotation,speedOrigin,1.5f,SpriteEffects.None,0f);

        spriteBatch.End();
    }

    private void DrawThirdPerson(SpriteBatch spriteBatch)
    {
        spriteBatch.Begin();

        spriteBatch.DrawString(_font, $"Speed: {(int)MathF.Abs(vehiclecontroller.State.CurrentSpeed)} km/h", new Vector2(20, 20), Color.White);
        spriteBatch.DrawString(_font, $"Gear: {vehiclecontroller.State.CurrentGear}", new Vector2(20, 50), Color.White);
        spriteBatch.DrawString(_font, $"RPM: {(int)vehiclecontroller.State.EngineRPM}", new Vector2(20, 80), Color.White);
        spriteBatch.DrawString(_font, $"Transmission: {vehiclecontroller.Transmission}", new Vector2(20, 110), Color.White);
        spriteBatch.DrawString(_font, $"View: {vehiclecontroller.ViewMode}", new Vector2(20, 140), Color.White);
        spriteBatch.DrawString(_font, $"NOS: {(int)vehiclecontroller.State.NitrousAmount}%", new Vector2(20, 170), vehiclecontroller.State.NitrousEnabled ? Color.HotPink : Color.Gray);
        spriteBatch.DrawString(_font, $"Weather: {_weather.CurrentWeather}", new Vector2(20, 200), Color.Yellow);

        spriteBatch.End();
    }

    #endregion
    #endregion
}

/*
 
 TODO:

    - Implement turning view left or right from mouse input
    - Don't allow viewing left or right to turn the car, make it view left or right in the car
 
 */