namespace Synthwave.Core.Classes.Vehicle;

public class NitrousSystem
{
    #region Properties
    public float Amount = 100f;
    public bool Active;
    #endregion

    #region Methods
    public float GetMultiplier()
    {
        if (!Active || Amount <= 0f) return 1f;

        Amount -= 25f * 0.016f;
        return 1.8f;
    }
    #endregion
}