using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Core.Physics.Collision;

public class SpatialGrid
{
    private readonly Dictionary<Point, List<MeshCollider>> _cells = [];

    public int CellSize = 10;

    private Point GetCell(Vector3 pos) => new Point((int)(pos.X / CellSize), (int)(pos.Z / CellSize));
}
