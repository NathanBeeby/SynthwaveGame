using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Particles;

public class WindField : ParticleField
{
    public Vector3 Direction;
    public float Strength;

    public override Vector3 Apply(Vector3 position, Vector3 velocity) => velocity + Direction * Strength;
}
