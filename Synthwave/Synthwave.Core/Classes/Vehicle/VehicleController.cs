using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Synthwave.Core.Classes.Core;
using Synthwave.Core.Classes.Core.Enums;
using Synthwave.Core.Classes.Core.Input;
using Synthwave.Core.Classes.World;
using Synthwave.Core.Classes.World.Weather;
using System;

namespace Synthwave.Core.Classes.Vehicle;

public class VehicleController(Camera3D camera = null)
{
    #region Properties
    public Vector3 Position = Vector3.Zero;
    public float Yaw;

    public CameraMode ViewMode = CameraMode.FirstPerson;
    public TransmissionMode Transmission = TransmissionMode.Automatic;


    public VehicleState State = new();
    public VehiclePhysics Physics = new();
    public Engine Engine = new();
    public Transmission TransmissionSystem = new();
    public Tyres Tyres = new();
    public FuelSystem Fuel = new();
    public NitrousSystem NOS = new();
    public DamageSystem Damage = new();

    public WeatherSystem _weather;
    private TerrainSystem _terrain;
    public Camera3D _camera = camera;
    #endregion

    #region Update
    public void Update(GameTime gameTime, InputHandler input, WeatherSystem weather, TerrainSystem terrain = null)
    {
        float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

        _weather = weather;
        _terrain = terrain;

        UpdateVehicleInput(input, dt);
        UpdateViewModel(input);
        UpdateTransmission(input);
        UpdateNitrous(input);
    }

    private void UpdateVehicleInput(InputHandler input, float dt)
    {
        float throttle = input.IsKeyDown(Keys.W) ? 1f : 0f;
        float brake = input.IsKeyDown(Keys.Space) ? 1f : 0f;
        float steer = (input.IsKeyDown(Keys.A) ? 1f : 0f) - (input.IsKeyDown(Keys.D) ? 1f : 0f);

        Engine.Throttle = throttle;

        UpdateVehicleSystem(brake, steer, dt);
    }

    private void UpdateVehicleSystem(float brake, float steer, float dt)
    {
        Fuel.Consume(Engine.Throttle, Engine.RPM, dt);

        float nitroMult = NOS.GetMultiplier();
        float engineTorque = Engine.GetTorque(nitroMult, Damage.Engine);
        float gearRatio = TransmissionSystem.GetRatio();
        float engineForce = engineTorque * gearRatio * 12f; // scale to world force

        UpdateWeatherAndTerrainGrip(engineForce, brake, steer, dt);
    }

    private void UpdateWeatherAndTerrainGrip(float engineForce, float brake, float steer, float dt)
    {
        float hydro = (_terrain?.GetWaterLevel(Physics.Position.X, Physics.Position.Z) ?? 0f) * _weather.HydroplaningFactor;
        UpdateTerrainAndWeatherEffects(hydro);
        UpdateEngineState(engineForce, brake * 8000f, steer * _weather.SteeringMultiplier, dt);
        UpdateHydroplaneLateralEffect(hydro);
        UpdateYawSync();
        UpdateCameraFeedback(dt);
    }

    private void UpdateTerrainAndWeatherEffects(float hydro)
    {
        float weatherGrip = _weather.FrictionMultiplier * (1f - hydro);
        float tyreGrip = Tyres.GetSideGrip(Physics.Velocity.Length());
        Physics.Grip = MathHelper.Clamp(weatherGrip * tyreGrip, 0.1f, 1f);
        State.DriftFactor = 1f - Physics.Grip;
    }

    private void UpdateEngineState(float engineForce, float brakeForce, float steerInput, float dt)
    {
        Physics.ApplyForces((Fuel.Fuel != 0) ? engineForce : 0, brakeForce, steerInput, dt);
    }
    
    private void UpdateHydroplaneLateralEffect(float hydro)
    {
        if (hydro > 0.35f && Physics.Velocity.Length() > 15f)
        {
            Vector3 side = Physics.Right;
            Physics.Velocity += side * (Random.Shared.NextSingle() - 0.5f) * hydro * 6f;
        }
    }

    private void UpdateYawSync()
    {
        Yaw = Physics.Yaw;
        Position = Physics.Position;
    }

    private void UpdateCameraFeedback(float dt)
    {
        if (_camera != null)
        {
            float nosShake = NOS.Active ? 0.25f : 0f;
            _camera.ShakeAmount = nosShake + _weather.CameraShake;
            _camera.FovKick = MathHelper.Lerp(_camera.FovKick, NOS.Active ? 12f : 0f, dt * 5f);
        }
    }
    #endregion

    #region Gearbox
    private void UpdateViewModel(InputHandler input)
    {
        if (input.WasJustPressed(Keys.C)) ViewMode = ViewMode == CameraMode.FirstPerson ? CameraMode.ThirdPerson : CameraMode.FirstPerson;
    }

    private void UpdateTransmission(InputHandler input)
    {
        if (input.WasJustPressed(Keys.T))
            Transmission = Transmission == TransmissionMode.Automatic ? TransmissionMode.Manual : TransmissionMode.Automatic;

        if (Transmission == TransmissionMode.Manual)
        {
            if (input.WasJustPressed(Keys.O)) State.CurrentGear = Math.Min(State.CurrentGear + 1, 6);
            if (input.WasJustPressed(Keys.L)) State.CurrentGear = Math.Max(State.CurrentGear - 1, 1);
        }
        else
        {
            if (State.CurrentGear < 6 && State.CurrentSpeed > Data.GearMaxSpeeds[State.CurrentGear] * 0.9f) State.CurrentGear++;
            if (State.CurrentGear > 1 && State.CurrentSpeed < Data.GearMaxSpeeds[State.CurrentGear - 1] * 0.6f) State.CurrentGear--;
        }
    }
    #endregion

    #region NOS
    private void UpdateNitrous(InputHandler input)
    {
        if (input.WasJustPressed(Keys.Z) && State.NitrousAmount > 0f) State.NitrousEnabled = !State.NitrousEnabled;

        if (State.NitrousEnabled)
        {
            State.NitrousAmount -= 20f / 60f;
            if (State.NitrousAmount <= 0f)
            {
                State.NitrousAmount = 0f;
                State.NitrousEnabled = false;
            }
        }
        else
        {
            State.NitrousAmount = Math.Min(State.NitrousAmount + 0.1f, 100f);
        }
    }
    #endregion
}
