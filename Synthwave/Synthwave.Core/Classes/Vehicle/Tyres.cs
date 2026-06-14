using System;

namespace Synthwave.Core.Classes.Vehicle;


public class Tyres
{
    #region Properties
    public float Grip = 1f;
    #endregion

    #region Methods
    public float GetSideGrip(float speed)
    {
        return Grip * (1f - MathF.Min(0.8f, speed / 200f));
    }
    #endregion
}