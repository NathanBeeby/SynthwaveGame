using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Vehicle;

public class VehicleState
{
    public Vector3 Velocity;
    public Vector3 Acceleration;
    public Vector3 AngularVelocity;

    public float Mass = 14000f;
    public float InverseMass => 1f / Mass;

    public float EngineRPM = 800f;
    public float EngineBraking = 8f;
    public float WheelBase = 2.7f;
    public float SteeringLimit = MathHelper.ToRadians(35f);
    public float SteeringInput;

    public int CurrentGear = 1;
    public bool ReverseGear = false;

    public float CurrentSpeed = 0f;
    public float MaxSpeed = 220f;

    public float Grip; // 0-1 normal traction
    public float DriftFactor = 0.90f; // 0-1

    public float BrakeForce = 60f;

    public float SteeringAngle = 0f;

    public float VehicleMass = 1400f;

    public bool NitrousEnabled = false;
    public float NitrousMultiplier = 2.5f;
    public float NitrousAmount = 100f;

    public float Fuel = 20; // In Ltrs
    public float EngineHealth = 1f;
    public float GearboxHealth = 1f;
    public float TyreWear = 0f;

    public bool IsRunning => EngineHealth > 0.05f && Fuel > 0f;
}