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

    // Interior resolution (world texture size)
    private const int InteriorWidth = 1500;
    private const int InteriorHeight = 650;

    // View window (what player sees)
    private const int ViewWidth = 800;
    private const int ViewHeight = 600;
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
        _rpmNeedle = content.Load<Texture2D>("2DSprites/SpeedNeedle");
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        UpdateSteering(dt);
        UpdateGauges(dt);
    }

    private void UpdateSteering(float dt)
    {
        float steerInput = vehiclecontroller.State.SteeringInput;
        float targetWheel = steerInput * MaxWheelRotation;

        _wheelRotation = MathHelper.Lerp(_wheelRotation, targetWheel, dt * 10f);
    }

    private void UpdateGauges(float dt)
    {
        float speedPercent = MathHelper.Clamp(vehiclecontroller.State.CurrentSpeed / MaxSpeed, 0f, 1f);
        float rpmPercent = MathHelper.Clamp(vehiclecontroller.State.EngineRPM / MaxRPM, 0f, 1f);

        _speedRotation = MathHelper.Lerp(_speedRotation,MapGauge(speedPercent),dt * 8f);
        _rpmRotation = MathHelper.Lerp(_rpmRotation,MapGauge(rpmPercent),dt * 8f);
    }

    private float MapGauge(float percent) => MathHelper.Lerp(NeedleStartAngle, NeedleEndAngle, percent);
    #endregion

    #region Draw

    public void Draw(SpriteBatch spriteBatch)
    {
        if (vehiclecontroller.ViewMode == CameraMode.FirstPerson)  DrawFirstPerson(spriteBatch);
        else DrawThirdPerson(spriteBatch);
    }

    private void DrawWheel(SpriteBatch spriteBatch, Vector2 dashboardOffset, int screenW, int screenH)
    {
        Vector2 wheelPos = new(screenW * 0.5f, screenH - 60f);
        wheelPos += dashboardOffset;
        Vector2 wheelOrigin = new(_wheel.Width * 0.5f, _wheel.Height * 0.5f);

        spriteBatch.Draw(_wheel, wheelPos, null, Color.White, _wheelRotation, wheelOrigin, 2f, SpriteEffects.None, 0f);

    }

    private void DrawRPMNeedle(SpriteBatch spriteBatch, Vector2 dashboardOffset, int screenW, int screenH)
    {
        Vector2 rpmPos = new(screenW * 0.61f, screenH - 220f);
        rpmPos += dashboardOffset;
        Vector2 rpmOrigin = new(_rpmNeedle.Width * 0.5f, _rpmNeedle.Height * 0.9f);

        spriteBatch.Draw(_rpmNeedle, rpmPos, null, Color.White, _rpmRotation, rpmOrigin, 1.5f, SpriteEffects.None, 0f);

    }

    private void DrawSpeedNeedle(SpriteBatch spriteBatch, Vector2 dashboardOffset, int screenW, int screenH)
    {
        Vector2 speedPos = new(screenW * 0.38f, screenH - 220f);
        speedPos += dashboardOffset;
        Vector2 speedOrigin = new(_speedNeedle.Width * 0.5f, _speedNeedle.Height * 0.9f);

        spriteBatch.Draw(_speedNeedle, speedPos, null, Color.White, _speedRotation, speedOrigin, 1.5f, SpriteEffects.None, 0f);

    }

    private void DrawFirstPerson(SpriteBatch spriteBatch)
    {
        GraphicsDevice device = spriteBatch.GraphicsDevice;

        int screenW = device.Viewport.Width;
        int screenH = device.Viewport.Height;

        Vector2 camOffset = GetCameraLookOffset();

        const float maxOffsetX = 220f;
        const float maxOffsetY = 110f;

        int centerX = InteriorWidth / 2;
        int centerY = InteriorHeight / 2;

        int offsetX = centerX + (int)(camOffset.X * maxOffsetX);
        int offsetY = centerY + (int)(-camOffset.Y * maxOffsetY);

        offsetX = MathHelper.Clamp(offsetX, ViewWidth / 2, InteriorWidth - ViewWidth / 2);
        offsetY = MathHelper.Clamp(offsetY, ViewHeight / 2, InteriorHeight - ViewHeight / 2);

        Rectangle sourceRect = new(offsetX - ViewWidth / 2,offsetY - ViewHeight / 2,ViewWidth,ViewHeight);

        Rectangle screenRect = new(0, 0, screenW, screenH);

        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

        spriteBatch.Draw(_interior, screenRect, sourceRect, Color.White);
        Vector2 dashboardOffset = new(-(offsetX - centerX),-(offsetY - centerY));

        //Vector2 dashboardOffset = new(-(camOffset.X * maxOffsetX), (camOffset.Y * maxOffsetY));

        DrawWheel(spriteBatch, dashboardOffset, screenW, screenH);
        DrawRPMNeedle(spriteBatch, dashboardOffset, screenW, screenH);
        DrawSpeedNeedle(spriteBatch, dashboardOffset, screenW, screenH);

        spriteBatch.End();
    }

    private Vector2 GetCameraLookOffset()
    {
        if (_camera == null)
            return Vector2.Zero;

        float yaw = MathHelper.Clamp(_camera.HeadYaw / MathHelper.ToRadians(30f), -1f, 1f);
        float pitch = MathHelper.Clamp(_camera.Pitch / MathHelper.ToRadians(25f), -1f, 1f);

        return new Vector2(yaw, pitch);
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
}
/*
 
 TODO:

    - Implement turning view left or right from mouse input
    - Don't allow viewing left or right to turn the car, make it view left or right in the car
 
 */