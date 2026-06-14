using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Vehicle;

public class Engine
{
    #region Properties
    public float RPM = 800f;
    public float Redline = 7500f;
    public float Idle = 800f;
    public float MaxTorque = 450f;
    public float Throttle;
    #endregion

    #region Methods
    public float GetTorque(float nitrous, float damage)
    {
        float t = MaxTorque * Throttle;
        t *= nitrous;
        t *= (1f - damage);

        float rpmFactor = MathHelper.Clamp(RPM / Redline, 0f, 1f);

        return t * (1f - rpmFactor * 0.3f);
    }
    #endregion
}