namespace Synthwave.Core.Classes.Core;

public class EnvironmentLightingState
{
    public float TimeOfDay; // 0–24
    public bool IsNight => TimeOfDay < 6 || TimeOfDay > 19;

    public float NightFactor =>
        IsNight ? 1f : 0f;
}
