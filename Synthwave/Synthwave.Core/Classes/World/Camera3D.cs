using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;
using System.Diagnostics;

namespace Synthwave.Core.Classes.World;

public class Camera3D
{
    #region Properties
    public bool FlyMode;

    public float Yaw;
    public float Pitch;
    private float HeadYaw;
    public float EyeHeight = 3f;
    public float MouseSensitivity = 0.003f;
    public float NearPlane = 0.5f;
    public float FarPlane = 6000f;
    public float FlySpeed = 300f;

    public Vector3 Position;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }

    private MouseState _prevMouse;

    public VehicleController Vehicle;
    #endregion

    #region Constructor

    public Camera3D(GraphicsDevice device)
    {
        Vehicle = new VehicleController();
        Position = new Vector3(0, 10, 0);
        Yaw = 0;
        Pitch = -0.1f;
        Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(70), device.Viewport.AspectRatio, NearPlane, FarPlane);
        _prevMouse = Mouse.GetState();

        UpdateView();
    }

    #endregion

    #region Update

    public void Update(GameTime gameTime, InputHandler input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

#if DEBUG
        if (Debugger.IsAttached && input.WasJustPressed(Keys.F)) FlyMode = !FlyMode;
#endif

        if (FlyMode)
        {
            UpdateFlyMode(dt, input);
            UpdateView();

            return;
        }

        UpdateVehiclePosition(gameTime, input);
        UpdateMouseLook();
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
        HeadYaw -= dx * MouseSensitivity;
        Pitch -= dy * MouseSensitivity;
    }
    private void UpdateVehiclePosition(GameTime gameTime, InputHandler input)
    {
        Vehicle.Update(gameTime, input);
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
    }

    #endregion

    #region View
    private void UpdateFlyView()
    {
        Matrix rotation = Matrix.CreateRotationY(Yaw) * Matrix.CreateRotationX(Pitch);
        Vector3 lookFly = Vector3.Transform(Vector3.Forward, rotation);

        View = Matrix.CreateLookAt(Position, Position + lookFly, Vector3.Up);
    }

    private void UpdatePersonView()
    {
        float finalYaw = Yaw + HeadYaw;
        Matrix rotation = Matrix.CreateRotationY(finalYaw) * Matrix.CreateRotationX(Pitch);
        Vector3 look = Vector3.Transform(Vector3.Forward, rotation);

        if (Vehicle.ViewMode == CameraMode.FirstPerson)
        {
            Vector3 eye = Position + Vector3.Up * EyeHeight;
            View = Matrix.CreateLookAt(eye, eye + look, Vector3.Up);
        }
        else
        {
            Vector3 back = Vector3.Normalize(new Vector3(-look.X, 0, -look.Z));
            Vector3 cameraPos = Position + Vector3.Up * 5f + back * 12f;
            View = Matrix.CreateLookAt(cameraPos, Position + Vector3.Up * 2f, Vector3.Up);
        }
    }

    private void UpdateView()
    {
        if (FlyMode)
        {
            UpdateFlyView();
            return;
        }
        UpdatePersonView();
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
 */