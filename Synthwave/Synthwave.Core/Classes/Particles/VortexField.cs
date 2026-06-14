using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Particles;

public class VortexField : ParticleField
{
    public Vector3 Center;
    public float Strength;

    public override Vector3 Apply(Vector3 position, Vector3 velocity)
    {
        Vector3 dir = Vector3.Normalize(Center - position);
        Vector3 tangent = Vector3.Cross(dir, Vector3.Up);

        return velocity + tangent * Strength;
    }
}
