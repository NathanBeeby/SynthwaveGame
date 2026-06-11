using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using System;

namespace Synthwave.Core.Classes.World;

public class VehicleController
{
    #region Properties
    public Vector3 Position;
    public float Yaw;

    public CameraMode ViewMode = CameraMode.FirstPerson;
    public TransmissionMode Transmission = TransmissionMode.Automatic;

    public float EngineRPM;
    public float EngineBraking;
    public float WheelBase;
    public float SteeringLimit;

    public int CurrentGear = 1;
    public bool ReverseGear = false;

    public float CurrentSpeed = 0f;
    public float MaxSpeed = 220f;

    public float Acceleration = 25f;
    public float BrakeForce = 60f;

    public float SteeringAngle = 0f;

    public float VehicleMass = 1400f;

    public bool NitrousEnabled = false;
    public float NitrousMultiplier = 2.5f;
    public float NitrousAmount = 100f;

    public float DriftFactor = 0.90f;

    private Vector3 _velocity;
    #endregion

    #region Constructor
    public VehicleController()
    {
        Position = Vector3.Zero;

        CurrentGear = 1;
        EngineRPM = 800f;
        EngineBraking = 8f;
        WheelBase = 2.7f;
        SteeringLimit = MathHelper.ToRadians(35f);
        VehicleMass = 1400f;
        Acceleration = 25f;
        BrakeForce = 60f;
        DriftFactor = 0.90f;
        NitrousMultiplier = 2.5f;
        NitrousAmount = 100f;
    }
    #endregion

    #region Update

    public void Update(GameTime gameTime, InputHandler input)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
        UpdateViewModel(input);
        UpdateTransmission(input);
        UpdateNitrous(input);

        bool accelerate = input.IsKeyDown(Keys.W) || input.IsKeyDown(Keys.Up);
        bool reverse = input.IsKeyDown(Keys.S) || input.IsKeyDown(Keys.Down);
        bool brake = input.IsKeyDown(Keys.Space);

        float gearMax = Data.GearMaxSpeeds[CurrentGear];
        float multiplier = NitrousEnabled ? NitrousMultiplier : 1f;

        if (accelerate) CurrentSpeed += Acceleration * multiplier * dt;
        if (reverse) CurrentSpeed -= Acceleration * dt;

        if (brake)
        {
            if (CurrentSpeed > 0) CurrentSpeed -= BrakeForce * dt;
            else CurrentSpeed += BrakeForce * dt;
        }

        if (!accelerate && !reverse) CurrentSpeed *= 0.985f;

        CurrentSpeed = MathHelper.Clamp(CurrentSpeed, -30f, gearMax);

        float steeringInput = 0f;

        if (input.IsKeyDown(Keys.A) || input.IsKeyDown(Keys.Left)) steeringInput = 1f;
        if (input.IsKeyDown(Keys.D) || input.IsKeyDown(Keys.Right)) steeringInput = -1f;

        SteeringAngle = MathHelper.Lerp(SteeringAngle, steeringInput * 0.7f, dt * 5f);
        Yaw += SteeringAngle * (MathF.Abs(CurrentSpeed) / 50f) * dt;

        Vector3 forward = new(-MathF.Sin(Yaw), 0, -MathF.Cos(Yaw));
        Vector3 desiredVelocity = forward * CurrentSpeed;
        float traction = NitrousEnabled ? 0.3f : DriftFactor;

        _velocity = Vector3.Lerp(_velocity, desiredVelocity, traction * dt);
        Position += _velocity * dt;
    }

    #endregion

    #region Gearbox
    private void UpdateViewModel(InputHandler input)
    {
        if (input.WasJustPressed(Keys.C))
        {
            ViewMode = ViewMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
        }

    }
    private void UpdateTransmission(InputHandler input)
    {
        if (input.WasJustPressed(Keys.T))
        {
            Transmission = Transmission == TransmissionMode.Automatic ? TransmissionMode.Manual : TransmissionMode.Automatic;
        }

        if (Transmission == TransmissionMode.Manual)
        {
            if (input.WasJustPressed(Keys.O)) CurrentGear = System.Math.Min(CurrentGear + 1, 6);
            if (input.WasJustPressed(Keys.L)) CurrentGear = System.Math.Max(CurrentGear - 1, 1);
        }
        else
        {
            if (CurrentGear < 6 && CurrentSpeed > Data.GearMaxSpeeds[CurrentGear] * 0.90f) CurrentGear++;
            if (CurrentGear > 1 && CurrentSpeed < Data.GearMaxSpeeds[CurrentGear - 1] * 0.60f) CurrentGear--;
        }
    }

    #endregion

    #region NOS

    private void UpdateNitrous(InputHandler input)
    {
        if (input.WasJustPressed(Keys.Z))
        {
            if (NitrousAmount > 0) NitrousEnabled = !NitrousEnabled;
        }

        if (NitrousEnabled)
        {
            NitrousAmount -= 20f / 60f;

            if (NitrousAmount <= 0)
            {
                NitrousAmount = 0;
                NitrousEnabled = false;
            }
        }
        else
        {
            NitrousAmount = System.Math.Min(NitrousAmount + 0.1f, 100f);
        }
    }

    #endregion
}
