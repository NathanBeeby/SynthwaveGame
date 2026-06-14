using Microsoft.Xna.Framework;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public struct MeshCollider
{
    public Vector3[] Vertices;
    public int[] Indices;

    public Matrix Transform;

    public static Vector3 TransformVertex(Vector3 v, Matrix transform) => Vector3.Transform(v, transform);

    public static bool Intersects(SphereCollider sphere, MeshCollider mesh)
    {
        float radiusSq = sphere.Radius * sphere.Radius;

        foreach (var v in mesh.Vertices)
        {
            Vector3 worldV = Vector3.Transform(v, mesh.Transform);

            if (Vector3.DistanceSquared(worldV, sphere.Center) <= radiusSq)
                return true;
        }

        return false;
    }
}
