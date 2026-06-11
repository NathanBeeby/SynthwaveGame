using Microsoft.Xna.Framework;
using Synthwave.Core.Classes.Renderer;
using Synthwave.Core.Classes.World.Sky;

namespace Synthwave.Core.Classes.World;

public class LightingSystem
{
    #region Properties
    public Vector3 AmbientColor;
    #endregion

    #region Methods
    public void Update(SkySystem sky)
    {
        float sun = sky.GetSunIntensity();
        float moon = sky.GetMoonIntensity();

        Vector3 synthBase = new(0.9f, 0.2f, 0.8f);

        AmbientColor =
            sky.SkyColor * sun * 0.8f +
            new Vector3(0.2f, 0.1f, 0.3f) * moon +
            synthBase * 0.12f;
    }

    public void Apply(BloomRenderer renderer) => renderer.AmbientLight = AmbientColor;
    #endregion
}
