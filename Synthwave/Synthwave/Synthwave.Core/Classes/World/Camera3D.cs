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
    public Vector3 Position;
    public float Yaw;
    public float Pitch;

    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }

    public float MoveSpeed = 120f;
    public float TurnSpeed = 2.0f;
    public float MouseSensitivity = 0.003f;
    public float NearPlane = 0.5f;
    public float FarPlane = 6000f;
    public float EyeHeight = 3f;

    public bool FlyMode = false;   // toggled with F in debug
    public float FlySpeed = 300f;

    private MouseState _prevMouse;

    #endregion

    #region Constructor
    public Camera3D(GraphicsDevice device)
    {
        Position = new Vector3(0, 10f, 0);
        Yaw = 0f;
        Pitch = -0.10f;

        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(70f),
            device.Viewport.AspectRatio,
            NearPlane,
            FarPlane);

        _prevMouse = Mouse.GetState();
        UpdateView();
    }
    #endregion

    #region Update
    public void Update(GameTime gameTime, InputHandler input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        // ── Debug fly-mode toggle (only meaningful when debugger attached) ──
#if DEBUG
        if (Debugger.IsAttached && input.WasJustPressed(Keys.F))
            FlyMode = !FlyMode;
#endif

        // ── Mouse look ────────────────────────────────────────────────────
        MouseState mouse = Mouse.GetState();
        int dx = mouse.X - _prevMouse.X;
        int dy = mouse.Y - _prevMouse.Y;
        _prevMouse = mouse;

        Yaw -= dx * MouseSensitivity;
        Pitch -= dy * MouseSensitivity;
        Pitch = MathHelper.Clamp(Pitch, -1.3f, 0.4f);

        // ── Keyboard turn ─────────────────────────────────────────────────
        if (input.IsKeyDown(Keys.Left) || input.IsKeyDown(Keys.Q)) Yaw += TurnSpeed * dt;
        if (input.IsKeyDown(Keys.Right) || input.IsKeyDown(Keys.E)) Yaw -= TurnSpeed * dt;

        // ── Forward direction (FIXED: MonoGame Forward = (0,0,-1)) ─────────
        // Yaw=0 → looking down -Z.  Sin/Cos signs corrected accordingly.
        Vector3 forward = new(
            -MathF.Sin(Yaw),    // ← was +Sin, which reversed forward/back
            0f,
            -MathF.Cos(Yaw));   // ← was +Cos

        Vector3 right = Vector3.Cross(forward, Vector3.Up);  // left-hand rule not needed; explicit sign
        right.Normalize();

        float speed = FlyMode ? FlySpeed : MoveSpeed;

        // ── Horizontal movement ───────────────────────────────────────────
        if (input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up))
            Position += forward * speed * dt;
        if (input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down))
            Position -= forward * speed * dt;
        if (input.IsKeyDown(Keys.A))
            Position -= right * speed * dt;
        if (input.IsKeyDown(Keys.D))
            Position += right * speed * dt;

        // ── Vertical movement (fly mode only) ────────────────────────────
#if DEBUG
        if (Debugger.IsAttached && FlyMode)
        {
            if (input.IsKeyDown(Keys.R)) Position.Y += FlySpeed * dt;
            if (input.IsKeyDown(Keys.V)) Position.Y -= FlySpeed * dt;
        }
#endif

        UpdateView();
    }
    #endregion

    #region Internals
    public void SnapToTerrain(float terrainHeight)
    {
        if (!FlyMode)
        {
            Position.Y = terrainHeight + EyeHeight;
            UpdateView();
        }
    }

    private void UpdateView()
    {
        Matrix rot = Matrix.CreateRotationY(Yaw) * Matrix.CreateRotationX(Pitch);
        Vector3 look = Vector3.Transform(Vector3.Forward, rot);
        View = Matrix.CreateLookAt(Position, Position + look, Vector3.Up);
    }
    #endregion
}

/*
 W	Move Forward
S	Move Back
A	Move Left
D	Move Right
Q   Move Camera Left
E   Move Camera Right
R   Fly Up
V   Fly Down
 
←	Look Left
→	Look Right
↑	Look Up
↓	Look Down
 */