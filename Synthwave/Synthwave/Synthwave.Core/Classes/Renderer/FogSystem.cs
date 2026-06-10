using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Renderer;

public class FogSystem
{
    #region Properties
    public float FogStart = 400f;
    public float FogEnd = 2000f;
    public Vector3 FogColor = new(0.04f, 0f, 0.18f); // matches night sky
    #endregion

    #region Methods
    public Vector3 Apply(Vector3 color, float dist)
    {
        float t = MathHelper.Clamp((dist - FogStart) / (FogEnd - FogStart), 0f, 1f);
        return Vector3.Lerp(color, FogColor, t);
    }
    #endregion
}
