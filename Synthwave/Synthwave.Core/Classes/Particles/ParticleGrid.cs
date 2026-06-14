using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Particles;

public class ParticleGrid
{
    private Dictionary<Point, List<Particle>> _cells = new();

    public int CellSize = 10;

    private Point GetCell(Vector3 pos)
    {
        return new Point(
            (int)(pos.X / CellSize),
            (int)(pos.Z / CellSize)
        );
    }

}
