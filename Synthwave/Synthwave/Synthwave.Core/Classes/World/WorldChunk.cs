using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.World;

public class WorldChunk
{
    public Point Coord;
    public bool IsBuilt;

    public VertexBuffer TerrainVB;
    public IndexBuffer TerrainIB;

    public VertexBuffer RoadVB;
    public IndexBuffer RoadIB;

    /// <summary>Free GPU buffers so the chunk can be rebuilt or evicted.</summary>
    public void Dispose()
    {
        TerrainVB?.Dispose(); TerrainVB = null;
        TerrainIB?.Dispose(); TerrainIB = null;
        RoadVB?.Dispose(); RoadVB = null;
        RoadIB?.Dispose(); RoadIB = null;
        IsBuilt = false;
    }
}
