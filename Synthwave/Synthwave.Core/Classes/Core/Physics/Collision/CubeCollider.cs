using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public struct CubeCollider(Vector3 center, Vector3 halfSize)
{
    public Vector3 Center = center;
    public Vector3 HalfSize = halfSize;

    public readonly Vector3 Min => Center - HalfSize;
    public readonly Vector3 Max => Center + HalfSize;

    public readonly bool Intersects(CubeCollider other) => System.Math.Abs(Center.X - other.Center.X) <= HalfSize.X + other.HalfSize.X &&
               System.Math.Abs(Center.Y - other.Center.Y) <= HalfSize.Y + other.HalfSize.Y &&
               System.Math.Abs(Center.Z - other.Center.Z) <= HalfSize.Z + other.HalfSize.Z;
}