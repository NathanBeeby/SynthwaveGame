using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public struct SphereCollider(Vector3 center, float radius)
{
    public Vector3 Center = center;
    public float Radius = radius;

    public readonly bool Intersects(SphereCollider other)
    {
        float r = Radius + other.Radius;
        return Vector3.DistanceSquared(Center, other.Center) <= r * r;
    }
}