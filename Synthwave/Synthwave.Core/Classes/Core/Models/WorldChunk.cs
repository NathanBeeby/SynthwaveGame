using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Core.Models;

public class WorldChunk
{
    public Point Coord;
    public bool IsBuilt;
    public VertexBuffer TerrainVB;
    public IndexBuffer TerrainIB;
    public IndexBuffer GridIB;      // ← NEW: line-list for neon wireframe
    public VertexBuffer RoadVB;
    public IndexBuffer RoadIB;

    public void Dispose()
    {
        TerrainVB?.Dispose(); TerrainVB = null;
        TerrainIB?.Dispose(); TerrainIB = null;
        GridIB?.Dispose(); GridIB = null;   // ← NEW
        RoadVB?.Dispose(); RoadVB = null;
        RoadIB?.Dispose(); RoadIB = null;
        IsBuilt = false;
    }
}