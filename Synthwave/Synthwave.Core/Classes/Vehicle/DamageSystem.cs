using System;

namespace Synthwave.Core.Classes.Vehicle;

public class DamageSystem
{
    public float Engine = 0f;
    public float Gearbox = 0f;
    public float Body = 0f;

    public void AddImpact(float force)
    {
        Body += force * 0.0005f;
        Engine += force * 0.0002f;
        Gearbox += force * 0.0003f;

        Engine = MathF.Min(1f, Engine);
        Gearbox = MathF.Min(1f, Gearbox);
        Body = MathF.Min(1f, Body);
    }
}