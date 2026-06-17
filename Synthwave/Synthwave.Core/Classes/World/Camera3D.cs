using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.Menus.Core;
using Synthwave.Core.Classes.Vehicle;
using Synthwave.Core.Classes.World.Weather;
using System;
using System.Diagnostics;

namespace Synthwave.Core.Classes.World;

public class Camera3D
{
    #region Properties
    private ScreenManager screenManager;
    public bool FlyMode;

    public float Yaw;
    public float Pitch;
    public float HeadYaw;
    public float EyeHeight = 3f;
    public float MouseSensitivity = 0.003f;
    public float LookOffsetStrength = 0.08f;   // how far you can lean
    public float LookOffsetSpeed = 10f;        // smoothing
    public float NearPlane = 0.5f;
    public float FarPlane = 6000f;
    public float FlySpeed = 300f;
    public float ShakeAmount;
    public float FovKick;

    private Vector2 _lookOffset;
    private Vector2 _targetLookOffset;
    public Vector3 Position;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    private readonly float _aspectRatio;

    private MouseState _prevMouse;

    public VehicleController Vehicle;
    #endregion

    #region Constructor
    public Camera3D(GraphicsDevice device, ScreenManager Screen)
    {
        screenManager = Screen;
        Vehicle = screenManager.Services.GetService<VehicleController>();
        Position = new Vector3(0, 10, 0);
        Yaw = 0;
        Pitch = -0.1f;

        _aspectRatio = device.Viewport.AspectRatio;
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), _aspectRatio, NearPlane, FarPlane);
        _prevMouse = Mouse.GetState();

        UpdateView();
    }
    #endregion

    #region Update
    public void Update(GameTime gameTime, InputHandler input, WeatherSystem weather)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

#if DEBUG
        if (Debugger.IsAttached && input.WasJustPressed(Keys.F)) FlyMode = !FlyMode;
        if (Debugger.IsAttached && input.WasJustPressed(Keys.G)) weather.CycleWeather();
#endif

        if (FlyMode)
        {
            UpdateFlyMode(dt, input);
            UpdateView();
            return;
        }

        UpdateVehiclePosition(gameTime, input, weather);
        UpdateMouseLook();
        UpdateLookOffset(dt);
        UpdateHeadLook(input, dt);
        UpdateView();
    }
    #endregion

    private void UpdateHeadLook(InputHandler input, float dt)
    {
        if (input.IsKeyDown(Keys.Q)) HeadYaw += dt;
        if (input.IsKeyDown(Keys.E)) HeadYaw -= dt;

        HeadYaw = MathHelper.Clamp(HeadYaw, MathHelper.ToRadians(-30), MathHelper.ToRadians(30));
        Pitch = MathHelper.Clamp(Pitch, -1.3f, 0.4f);
    }

    private void UpdateMouseLook()
    {
        MouseState mouse = Mouse.GetState();

        int dx = mouse.X - _prevMouse.X;
        int dy = mouse.Y - _prevMouse.Y;

        _prevMouse = mouse;

        // normal rotation stays (look direction)
        HeadYaw += dx * MouseSensitivity;
        Pitch -= dy * MouseSensitivity;

        Pitch = MathHelper.Clamp(Pitch, -1.3f, 0.4f);

        // NEW: also drive positional offset
        _targetLookOffset.X = MathHelper.Clamp(_targetLookOffset.X + dx * 0.0015f, -LookOffsetStrength, LookOffsetStrength);
        _targetLookOffset.Y = MathHelper.Clamp(_targetLookOffset.Y + dy * 0.0015f, -LookOffsetStrength, LookOffsetStrength);
    }

    private void UpdateLookOffset(float dt)
    {
        float t = 1f - MathF.Exp(-LookOffsetSpeed * dt);

        _lookOffset = Vector2.Lerp(_lookOffset, _targetLookOffset, t);
    }

    private void UpdateVehiclePosition(GameTime gameTime, InputHandler input, WeatherSystem weather)
    {
        Vehicle.Update(gameTime, input, weather);
        Position = Vehicle.Position;
        Yaw = Vehicle.Yaw;
    }

    #region Fly Mode
    private void UpdateFlyMode(float dt, InputHandler input)
    {
        MouseState mouse = Mouse.GetState();

        int dx = mouse.X - _prevMouse.X;
        int dy = mouse.Y - _prevMouse.Y;

        _prevMouse = mouse;

        Yaw -= dx * MouseSensitivity;
        Pitch -= dy * MouseSensitivity;

        Pitch = MathHelper.Clamp(Pitch, -1.5f, 1.5f);

        Matrix rotation = Matrix.CreateRotationY(Yaw) * Matrix.CreateRotationX(Pitch);
        Vector3 forward = Vector3.Transform(Vector3.Forward, rotation);
        Vector3 right = Vector3.Transform(Vector3.Right, rotation);

        if (input.IsKeyDown(Keys.W)) Position += forward * FlySpeed * dt;
        if (input.IsKeyDown(Keys.S)) Position -= forward * FlySpeed * dt;
        if (input.IsKeyDown(Keys.A)) Position -= right * FlySpeed * dt;
        if (input.IsKeyDown(Keys.D)) Position += right * FlySpeed * dt;
        if (input.IsKeyDown(Keys.R)) Position += Vector3.Up * FlySpeed * dt;
        if (input.IsKeyDown(Keys.V)) Position -= Vector3.Up * FlySpeed * dt;
    }
    #endregion

    #region Terrain
    public void SnapToTerrain(float terrainHeight)
    {
        if (FlyMode) return;

        Vehicle.Position.Y = terrainHeight;
        Position = Vehicle.Position;
        UpdateView();
    }
    #endregion

    #region View
    private void UpdateFlyView()
    {
        Matrix rotation = Matrix.CreateRotationY(Yaw) * Matrix.CreateRotationX(Pitch);
        Vector3 lookFly = Vector3.Transform(Vector3.Forward, rotation);

        // Apply camera shake
        Vector3 shakeOffset = new((float)(Random.Shared.NextDouble() - 0.5f) * ShakeAmount, (float)(Random.Shared.NextDouble() - 0.5f) * ShakeAmount, 0);
        View = Matrix.CreateLookAt(Position + shakeOffset, Position + lookFly + shakeOffset, Vector3.Up);
    }

    private void UpdatePersonView()
    {
        float finalYaw = Yaw + HeadYaw;

        Matrix rotation =Matrix.CreateRotationY(finalYaw) *Matrix.CreateRotationX(Pitch);

        Vector3 look = Vector3.Transform(Vector3.Forward, rotation);
        Vector3 right = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(finalYaw));
        Vector3 up = Vector3.Up;
        Vector3 eyeOffset =right * _lookOffset.X +up * -_lookOffset.Y;
        Vector3 shakeOffset = new((float)(Random.Shared.NextDouble() - 0.5f) * ShakeAmount,(float)(Random.Shared.NextDouble() - 0.5f) * ShakeAmount,0);

        if (Vehicle.ViewMode == CameraMode.FirstPerson)
        {
            Vector3 eye =Position +Vector3.Up * EyeHeight +eyeOffset + shakeOffset;
            View = Matrix.CreateLookAt(eye,eye + look,Vector3.Up);
        }
        else
        {
            Vector3 back = Vector3.Normalize(new Vector3(-look.X, 0, -look.Z));
            Vector3 cameraPos = Position + Vector3.Up * 5f + back * 12f + eyeOffset + shakeOffset;

            View = Matrix.CreateLookAt(cameraPos,Position + Vector3.Up * 2f + shakeOffset,Vector3.Up);
        }
    }

    private void UpdateView()
    {
        if (FlyMode) UpdateFlyView();
        else UpdatePersonView();

        // Optional FOV kick for NOS — use the fixed aspect ratio captured at
        // construction. Never derive it from the previous Projection matrix.
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70 + FovKick), _aspectRatio, NearPlane, FarPlane);
    }
    #endregion
}

/*
Key	Action
W	        Accelerate
S	        Reverse
A	        Steer Left
D	        Steer Right
Space	    Brake
Z	        Toggle NOS
C	        First / Third Person
T	        Automatic / Manual
O	        Gear Up
L           Gear Down
Q	        Look Left
E	        Look Right
F	        Debug Fly
R	        Fly Up
V	        Fly Down
G           Cycle Weather
 */