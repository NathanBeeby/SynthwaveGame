using System;

namespace Synthwave.Core.Classes.Vehicle;

public class FuelSystem
{
    #region Properties
    public float Fuel = 60f;
    #endregion

    #region Methods
    public void Consume(float throttle, float rpm, float dt)
    {
        Fuel -= (0.0002f + throttle * rpm * 0.0000005f) * dt;
        Fuel = MathF.Max(0f, Fuel);
    }
    #endregion
}