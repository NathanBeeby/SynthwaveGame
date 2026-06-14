using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Particles;

public abstract class ParticleField
{
    public abstract Vector3 Apply(Vector3 position, Vector3 velocity);
}
