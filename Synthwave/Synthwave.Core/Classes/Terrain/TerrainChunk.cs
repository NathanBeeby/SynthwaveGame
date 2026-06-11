using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Synthwave.Core.Classes.Terrain;
using Synthwave.Core.Classes.World;
using System.Collections.Generic;

namespace Synthwave.Core.Classes.Models;

public class TerrainChunk
{
    #region Properties
    public const int Size = 64;
    public Point Coordinate { get; }
    public VertexPositionColor[] Vertices { get; }
    private readonly HeightMap map;
    #endregion

    #region Constructor
    public TerrainChunk(Point coord, HeightMap map)
    {
        Coordinate = coord;
        this.map = map;
        Vertices = Build();
    }
    #endregion

    #region Methods
    private VertexPositionColor[] Build()
    {
        List<VertexPositionColor> verts = [];

        int startX = Coordinate.X * Size;
        int startZ = Coordinate.Y * Size;

        for (int z = 0; z < Size - 1; z++)
        {
            for (int x = 0; x < Size - 1; x++)
            {
                int x0 = startX + x;
                int z0 = startZ + z;
                int x1 = x0 + 1;
                int z1 = z0 + 1;

                float h00 = map.GetHeight(x0, z0) * 80f;
                float h10 = map.GetHeight(x1, z0) * 80f;
                float h01 = map.GetHeight(x0, z1) * 80f;
                float h11 = map.GetHeight(x1, z1) * 80f;

                Color c = GetColor(map.GetTerrainType(x0, z0));

                // TRIANGLE 1
                verts.Add(new VertexPositionColor(new Vector3(x0, h00, z0), c));
                verts.Add(new VertexPositionColor(new Vector3(x1, h10, z0), c));
                verts.Add(new VertexPositionColor(new Vector3(x0, h01, z1), c));

                // TRIANGLE 2
                verts.Add(new VertexPositionColor(new Vector3(x1, h10, z0), c));
                verts.Add(new VertexPositionColor(new Vector3(x1, h11, z1), c));
                verts.Add(new VertexPositionColor(new Vector3(x0, h01, z1), c));
            }
        }

        return [.. verts];
    }

    private static Color GetColor(TerrainType type)
    {
        return type switch
        {
            TerrainType.Water => Color.CornflowerBlue,
            TerrainType.Road => Color.Yellow,
            _ => Color.LimeGreen
        };
    }
    #endregion
}