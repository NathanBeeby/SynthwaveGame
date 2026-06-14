namespace Synthwave.Core.Classes.Vehicle;

public class Transmission
{
    #region Properties
    public float[] Ratios = { -3.2f, 0f, 3.2f, 2.1f, 1.5f, 1.1f, 0.8f };
    public int Gear = 2;
    public float FinalDrive = 3.5f;
    #endregion

    #region Methods
    public float GetRatio() => Ratios[Gear] * FinalDrive;
    #endregion
}