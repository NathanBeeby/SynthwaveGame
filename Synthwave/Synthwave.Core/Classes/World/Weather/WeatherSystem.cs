using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Core.Enums;
using System;

namespace Synthwave.Core.Classes.World.Weather;

public class WeatherSystem
{
    #region Properties
    public WeatherType CurrentWeather = WeatherType.Default;
    private WeatherType _targetWeather = WeatherType.Default;

    public float TransitionSpeed = 0.2f;
    private float _blend = 1f;

    public float FrictionMultiplier { get; private set; } = 1f;
    public float SteeringMultiplier { get; private set; } = 1f;
    public float Visibility { get; private set; } = 1f;
    public float CameraShake { get; private set; } = 0f;
    public float ParticleIntensity { get; private set; }
    public float HydroplaningFactor { get; private set; } = 0f;

    public float RainAmount { get; private set; } = 0f;  // <-- NEW
    public float SnowAmount { get; private set; } = 0f;  // <-- NEW

    public Color AmbientTint { get; private set; }
    #endregion

    #region Methods
    public void SetWeather(WeatherType type) => ForceWeather(type);

    public void CycleWeather()
    {
        int next = ((int)CurrentWeather + 1) % Enum.GetValues(typeof(WeatherType)).Length;
        SetWeather((WeatherType)next);
    }

    public void ForceWeather(WeatherType type)
    {
        CurrentWeather = type;
        _targetWeather = type;
        _blend = 1f;
        ApplyWeather(type);
    }

    public void Update(GameTime time)
    {
        float dt = (float)time.ElapsedGameTime.TotalSeconds;

        if (CurrentWeather != _targetWeather)
        {
            _blend -= dt * TransitionSpeed;
            if (_blend <= 0f)
            {
                CurrentWeather = _targetWeather;
                _blend = 1f;
            }
        }

        ApplyWeather(CurrentWeather);
    }

    public void ApplyToEffect(Effect effect, float time, Vector3 cameraPos)
    {
        effect.Parameters["RainAmount"]?.SetValue(RainAmount);
        effect.Parameters["SnowAmount"]?.SetValue(SnowAmount);
        effect.Parameters["FogDensity"]?.SetValue(CurrentWeather == WeatherType.Fog ? 2.0f : 0.05f);
        effect.Parameters["WindStrength"]?.SetValue(1.0f);
        effect.Parameters["Time"]?.SetValue(time);
        effect.Parameters["CameraPosition"]?.SetValue(cameraPos);
    }

    private void ApplyWeather(WeatherType w)
    {
        // Reset first
        RainAmount = 0f;
        SnowAmount = 0f;

        switch (w)
        {
            case WeatherType.Default:
                FrictionMultiplier = 1f;
                SteeringMultiplier = 1f;
                Visibility = 1f;
                CameraShake = 0f;
                HydroplaningFactor = 0f;
                break;

            case WeatherType.Rain:
                FrictionMultiplier = 0.85f;
                SteeringMultiplier = 0.9f;
                Visibility = 0.85f;
                CameraShake = 0.05f;
                HydroplaningFactor = 0.2f;
                RainAmount = 0.6f;  // <-- set property
                break;

            case WeatherType.HeavyRain:
                FrictionMultiplier = 0.7f;
                SteeringMultiplier = 0.75f;
                Visibility = 0.65f;
                CameraShake = 0.15f;
                HydroplaningFactor = 0.5f;
                RainAmount = 1f;
                break;

            case WeatherType.Sleet:
                FrictionMultiplier = 0.55f;
                SteeringMultiplier = 0.6f;
                Visibility = 0.6f;
                CameraShake = 0.2f;
                HydroplaningFactor = 0.6f;
                RainAmount = 0.5f;  // partial rain/sleet
                SnowAmount = 0.5f;
                break;

            case WeatherType.Snow:
                FrictionMultiplier = 0.6f;
                SteeringMultiplier = 0.7f;
                Visibility = 0.7f;
                CameraShake = 0.05f;
                HydroplaningFactor = 0.4f;
                SnowAmount = 1f;
                break;

            case WeatherType.ArridHeat:
                FrictionMultiplier = 1.1f;
                SteeringMultiplier = 1.05f;
                Visibility = 0.9f;
                CameraShake = 0.02f;
                HydroplaningFactor = 0f;
                break;

            case WeatherType.Fog:
                FrictionMultiplier = 1f;
                SteeringMultiplier = 0.95f;
                Visibility = 0.4f;
                CameraShake = 0.03f;
                HydroplaningFactor = 0f;
                break;
        }
    }
    #endregion
}

/*
Proper volumetric rain system
world-space rain particles
wind direction
windshield splash shader
Snow accumulation system
cars + ground slowly turning white
True volumetric fog
distance-based ray marching fog
 */