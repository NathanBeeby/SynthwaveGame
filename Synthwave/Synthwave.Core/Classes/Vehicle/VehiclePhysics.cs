using Microsoft.Xna.Framework;
using System;

namespace Synthwave.Core.Classes.Vehicle;

public class VehiclePhysics
{
    #region Properties

    public float Yaw;
    public float Mass = 1400f;
    public float DragCoefficient = 0.425f;
    public float RollingResistance = 12f;
    public float Grip = 1f;

    public Vector3 Position;
    public Vector3 Velocity;

    #endregion

    #region Derived Vectors (SAFE)

    public Vector3 Forward
    {
        get
        {
            if (float.IsNaN(Yaw) || float.IsInfinity(Yaw))
                Yaw = 0f;

            Vector3 fwd = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Yaw));

            return IsValid(fwd) ? fwd : Vector3.Forward;
        }
    }

    public Vector3 Right
    {
        get
        {
            Vector3 r = Vector3.Cross(Vector3.Up, Forward);

            if (!IsValid(r) || r.LengthSquared() < 0.0001f)
                return Vector3.Right;

            return Vector3.Normalize(r);
        }
    }

    #endregion

    #region Main Physics

    public void ApplyForces(float engineForce, float brakeForce, float steering, float dt)
    {
        dt = Safe(dt);

        // -----------------------------
        // HARD SANITY CHECK (CRITICAL)
        // -----------------------------
        if (!IsValid(Velocity) || !IsValid(Position))
        {
            Velocity = Vector3.Zero;
            Position = Vector3.Zero;
            Yaw = 0f;
        }

        Vector3 fwd = Forward;
        Vector3 right = Right;

        // -----------------------------
        // Velocity decomposition
        // -----------------------------
        float forwardSpeed = Safe(Vector3.Dot(Velocity, fwd));
        float sideSpeed = Safe(Vector3.Dot(Velocity, right));

        // -----------------------------
        // FORCE BUILDING
        // -----------------------------
        Vector3 force = Vector3.Zero;

        // Engine
        force += fwd * Safe(engineForce);

        // Brake (SAFE directional braking)
        Vector3 brakeDir = forwardSpeed >= 0f ? -fwd : fwd;
        force += brakeDir * Safe(brakeForce);

        // -----------------------------
        // Drag (v² safe)
        // -----------------------------
        float speed = Velocity.Length();

        if (speed > 0.0001f && !float.IsNaN(speed))
        {
            Vector3 dragDir = -Velocity / speed;
            force += dragDir * speed * speed * DragCoefficient;
        }

        // -----------------------------
        // Rolling resistance
        // -----------------------------
        if (speed > 0.0001f)
        {
            Vector3 rollDir = -Velocity / speed;
            force += rollDir * RollingResistance;
        }

        // -----------------------------
        // Side grip (drift control)
        // -----------------------------
        float sideGrip = Safe(Grip) * 6f;
        force += -right * sideSpeed * sideGrip;

        // -----------------------------
        // Integration
        // -----------------------------
        Vector3 accel = force / Math.Max(Mass, 1f);

        Velocity += accel * dt;
        Position += Velocity * dt;

        // -----------------------------
        // Steering (stable bicycle model)
        // -----------------------------
        float speedFactor = Math.Clamp(MathF.Abs(forwardSpeed) / 25f, 0f, 1f);

        float steerAmount = Safe(steering) * speedFactor * dt * 2.5f;
        Yaw = Safe(Yaw + steerAmount);

        // -----------------------------
        // FINAL SANITIZATION (CRITICAL)
        // -----------------------------
        Velocity = Sanitize(Velocity);
        Position = Sanitize(Position);
    }

    #endregion

    #region Safety

    public static float Safe(float v)
        => float.IsNaN(v) || float.IsInfinity(v) ? 0f : v;

    private Vector3 Sanitize(Vector3 v)
    {
        if (float.IsNaN(v.X) || float.IsInfinity(v.X)) v.X = 0;
        if (float.IsNaN(v.Y) || float.IsInfinity(v.Y)) v.Y = 0;
        if (float.IsNaN(v.Z) || float.IsInfinity(v.Z)) v.Z = 0;
        return v;
    }

    private bool IsValid(Vector3 v)
        => !float.IsNaN(v.X) && !float.IsNaN(v.Y) && !float.IsNaN(v.Z)
        && !float.IsInfinity(v.X) && !float.IsInfinity(v.Y) && !float.IsInfinity(v.Z);

    #endregion
}