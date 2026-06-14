using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public static class CollisionMath
{
    public static bool Intersects(SphereCollider sphere, CubeCollider box)
    {
        Vector3 closest = Vector3.Clamp(sphere.Center, box.Min, box.Max);

        float distSq = Vector3.DistanceSquared(sphere.Center, closest);

        return distSq <= sphere.Radius * sphere.Radius;
    }
}