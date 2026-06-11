using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Synthwave.Core.Classes.Core.Models;

public class WorldChunk
{
    #region Properties
    public Point Coord;
    public bool IsBuilt;

    public VertexBuffer TerrainVB;
    public IndexBuffer TerrainIB;

    public VertexBuffer RoadVB;
    public IndexBuffer RoadIB;
    #endregion

    #region Methods
    /// <summary>Free GPU buffers so the chunk can be rebuilt or evicted.</summary>
    public void Dispose()
    {
        TerrainVB?.Dispose(); TerrainVB = null;
        TerrainIB?.Dispose(); TerrainIB = null;
        RoadVB?.Dispose(); RoadVB = null;
        RoadIB?.Dispose(); RoadIB = null;
        IsBuilt = false;
    }
    #endregion
}
