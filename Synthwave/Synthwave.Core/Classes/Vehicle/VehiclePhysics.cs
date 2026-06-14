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
    public Vector3 Forward => Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Yaw));
    public Vector3 Right => Vector3.Cross(Vector3.Up, Forward);
    #endregion

    #region Methods
    public void ApplyForces(float engineForce,float brakeForce,float steering,float dt)
    {
        Vector3 fwd = Forward;

        // Split velocity
        float forwardSpeed = Vector3.Dot(Velocity, fwd);
        float sideSpeed = Vector3.Dot(Velocity, Right);

        // DRIFT CONTROL (key to feel)
        float sideGrip = Grip * 6f;

        Vector3 sideCorrection = -Right * sideSpeed * sideGrip;

        // ENGINE FORCE
        Vector3 force = fwd * engineForce;

        // BRAKING
        force += -fwd * brakeForce * MathF.Sign(forwardSpeed);

        // DRAG (v²)
        force += -Velocity * Velocity.Length() * DragCoefficient;

        // SIDE FRICTION (drift system)
        force += sideCorrection;

        // INTEGRATION
        Vector3 accel = force / Mass;

        Velocity += accel * dt;
        Position += Velocity * dt;

        // YAW (bicycle-lite model)
        Yaw += steering * (forwardSpeed / 25f) * dt;
    }
    #endregion
}