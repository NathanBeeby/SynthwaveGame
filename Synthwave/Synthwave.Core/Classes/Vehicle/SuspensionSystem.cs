using System;

namespace Synthwave.Core.Classes.Vehicle;

public enum DamperType { Linear, Digressive, TwinTube }
public enum SpringType { Linear, Progressive }

public class SuspensionSystem
{
    // Geometry / static params
    public float MotionRatio = 1.0f;      // wheel travel -> spring travel
    public float SpringRateNPerM = 200000f; // N/m (convert from lbf/in as needed)
    public float SpringPreloadN = 0f;     // preload force
    public SpringType SpringModel = SpringType.Linear;

    public float ReboundDamping = 3000f;  // N/(m/s)
    public float BumpDamping = 4000f;     // N/(m/s)
    public DamperType DamperModel = DamperType.Digressive;

    public float WheelUnsprungMassKg = 40f; // wheel+hub
    public float SprungMassKg = 300f;        // mass supported by this suspension corner
    public float WheelRadiusM = 0.33f;

    // Anti-roll / sway bar (per axle use combined halves)
    public float AntiRollStiffnessNPerM = 0f; // torque coupling between left/right

    // Limits
    public float MaxCompressionM = 0.15f;
    public float MaxExtensionM = 0.15f;
    public float BumpStopStiffnessNPerM = 1e6f;
    public float BumpStopThresholdM = 0.12f;

    // State
    public float WheelTravelM = 0f;       // positive = compression
    public float WheelVelocityMPerS = 0f; // positive = compressing
    public float SpringDeflectionM => WheelTravelM * MotionRatio;
    public float SpringVelocityMPerS => WheelVelocityMPerS * MotionRatio;

    // Derived/diagnostic
    public float CurrentSpringForceN => ComputeSpringForce(SpringDeflectionM);
    public float CurrentDamperForceN => ComputeDamperForce(SpringVelocityMPerS, WheelVelocityMPerS);

    // Constructor
    public SuspensionSystem(float motionRatio = 1f, float springRateNPerM = 200000f, float unsprungKg = 40f, float sprungKg = 300f)
    {
        MotionRatio = motionRatio;
        SpringRateNPerM = springRateNPerM;
        WheelUnsprungMassKg = unsprungKg;
        SprungMassKg = sprungKg;
    }

    // Spring model (linear or progressive)
    private float ComputeSpringForce(float deflectionM)
    {
        float k = SpringRateNPerM;
        if (SpringModel == SpringType.Progressive)
        {
            // simple progressive curve: effective rate rises with deflection^2
            k *= 1f + 3f * MathF.Pow(MathF.Max(0f, deflectionM) / 0.05f, 2);
        }
        return -k * deflectionM - SpringPreloadN; // compressive positive -> negative sign convention adjust if needed
    }

    // Damper model: asymmetric bump/rebound, optional digressive effect
    private float ComputeDamperForce(float springVel, float wheelVel)
    {
        // relative velocity across damper = springVel (since motion ratio applied)
        float v = springVel;
        float c = v >= 0 ? BumpDamping : ReboundDamping; // positive compressing = bump
        if (DamperModel == DamperType.Digressive)
        {
            // digressive: high small-velocity damping, flattens at high v
            float smallVelFactor = 1f + 10f * MathF.Exp(-MathF.Abs(v) * 5f);
            c *= smallVelFactor;
        }
        return -c * v; // resists motion
    }

    // Per-simulation tick: integrate wheel/sprung dynamics (semi-implicit Euler)
    // roadInput = road penetration relative to nominal ride height (positive into wheel)
    // dt in seconds, chassisAccelVert is approximation of sprung mass vertical accel from other systems
    public (float wheelNormalForceN, float chassisForceOnSprungN) SimulateTick(float dt, float roadInputM, float chassisVerticalAccelMPerS2 = 0f, float lateralLoadTransferN = 0f)
    {
        // Compute desired wheel position from road: wheelbody tries to follow road + suspension deflection
        float wheelContactPoint = roadInputM; // simplification; for bump, roadInput positive compresses wheel

        // Update wheel velocity using unsprung mass dynamics: F = m*a
        // Forces on wheel: tire normal (from spring + damper), gravity on unsprung, tire contact reaction assumed at ground
        float springForce = ComputeSpringForce(SpringDeflectionM);
        float damperForce = ComputeDamperForce(SpringVelocityMPerS, WheelVelocityMPerS);

        // Bump stop
        float bumpForce = 0f;
        if (WheelTravelM > BumpStopThresholdM)
        {
            float over = WheelTravelM - BumpStopThresholdM;
            bumpForce = -BumpStopStiffnessNPerM * over;
        }

        // Total force transmitted to sprung mass (positive upward)
        float totalUpwardOnUnsprung = -(springForce + damperForce + bumpForce); // sign conventions: springForce negative when compressed
        // Wheel normal = force pressing tyre to ground = totalUpwardOnUnsprung + lateralLoadTransfer adjustment
        float wheelNormal = MathF.Max(0f, totalUpwardOnUnsprung + lateralLoadTransferN);

        // Integrate wheel travel velocity and position towards road (very simplified contact model)
        // If wheel in contact, wheel travel adjusts so wheel axle position = road + wheelRadius - deflection
        // Here we approximate acceleration of unsprung mass
        float gravity = 9.81f;
        float unsprungAcc = (totalUpwardOnUnsprung - WheelUnsprungMassKg * gravity) / WheelUnsprungMassKg;
        WheelVelocityMPerS += unsprungAcc * dt;
        WheelTravelM += WheelVelocityMPerS * dt;

        // Clamp travel
        WheelTravelM = Math.Clamp(WheelTravelM, -MaxExtensionM, MaxCompressionM);
        if (WheelTravelM <= -MaxExtensionM) WheelVelocityMPerS = 0f;
        if (WheelTravelM >= MaxCompressionM) WheelVelocityMPerS = 0f;

        // Reaction on sprung mass (equal and opposite)
        float chassisForce = -(springForce + damperForce + bumpForce); // upward on sprung mass

        // Simple pitch contribution: chassis vertical accel reduces effective force
        chassisForce += SprungMassKg * chassisVerticalAccelMPerS2;

        return (wheelNormal, chassisForce);
    }

    // Utility helpers
    public float CalculateRideRateNPerM()
    {
        // ride rate at wheel = springRate * motionRatio^2
        return SpringRateNPerM * MotionRatio * MotionRatio;
    }

    public float CalculateNaturalFrequencyHz()
    {
        float rideRate = CalculateRideRateNPerM();
        float m = SprungMassKg;
        return (float)(1.0 / (2.0 * Math.PI) * Math.Sqrt(rideRate / m));
    }

    // Tuning: set linear spring from lbf/in
    public void SetSpringRateLbfPerIn(float lbfPerIn)
    {
        // 1 lbf/in = 175.12677 N/m
        SpringRateNPerM = lbfPerIn * 175.12677f;
    }

    // Damage / modes
    public void DamageCompressionStop(float newThresholdM, float newStiffnessNPerM)
    {
        BumpStopThresholdM = newThresholdM;
        BumpStopStiffnessNPerM = newStiffnessNPerM;
    }
}